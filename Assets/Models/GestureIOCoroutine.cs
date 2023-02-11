using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Barracuda;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

public class GestureIOCoroutine : MonoBehaviour
{
    public NNModel modelAsset;
    private Model m_RuntimeModel;
    private IWorker m_Worker;
    private MixedRealityPose[] _jointPoses;
    private MixedRealityPose referencePose;

    [SerializeField] private ObjectEventReference handObject;
    [SerializeField] private Handedness _hand;
    [SerializeField] private DynamicGestureValidator dynamicGestureValidator;
    
    private bool handActive;
    private bool validThisFrame = false;

    public List<string> inputlabels;
    public List<string> outputlabels;
    public static readonly TrackedHandJoint[] _sortedJointIDs =
    {
        TrackedHandJoint.IndexDistalJoint,
        TrackedHandJoint.IndexKnuckle,
        TrackedHandJoint.IndexMiddleJoint,
        TrackedHandJoint.IndexTip,
        TrackedHandJoint.MiddleDistalJoint,
        TrackedHandJoint.MiddleKnuckle,
        TrackedHandJoint.MiddleMiddleJoint,
        TrackedHandJoint.MiddleTip,
        TrackedHandJoint.Palm,
        TrackedHandJoint.PinkyDistalJoint,
        TrackedHandJoint.PinkyKnuckle,
        TrackedHandJoint.PinkyMiddleJoint,
        TrackedHandJoint.PinkyTip,
        TrackedHandJoint.RingDistalJoint,
        TrackedHandJoint.RingKnuckle,
        TrackedHandJoint.RingMiddleJoint,
        TrackedHandJoint.RingTip,
        TrackedHandJoint.ThumbDistalJoint,
        TrackedHandJoint.ThumbProximalJoint,
        TrackedHandJoint.ThumbTip,
        TrackedHandJoint.Wrist
    };
    private Tensor input;
    private Tensor output;

    private Dictionary<string, float> features = new()
    {
        {GestureRecorder.D_IndexTip_MiddleTip,0},
        {GestureRecorder.D_MiddleTip_RingTip,0},
        {GestureRecorder.D_Palm_IndexTip,0},
        {GestureRecorder.D_Palm_MiddleTip,0},
        {GestureRecorder.D_Palm_PinkyTip,0},
        {GestureRecorder.D_Palm_RingTip,0},
        {GestureRecorder.D_Palm_ThumbTip,0},
        {GestureRecorder.D_RingTip_PinkyTip,0},
        {GestureRecorder.D_Thumbtip_Indextip,0},
        /*{GestureRecorder.Palm_Rot_W,0},
        {GestureRecorder.Palm_Rot_X,0},
        {GestureRecorder.Palm_Rot_Y,0},
        {GestureRecorder.Palm_Rot_Z,0}*/
    };
    private Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new();
    float[] z_exp;
    float[] softmax;
    float sum_z_exp;
    float[] list;
    public float[] outputValues = new float[8];
    
    private void Start()
    {
        
        
        for (var index = 0; index < GestureRecorder._jointIDs.Length; index++)
        {
            var jointid = GestureRecorder._jointIDs[index];
            jointPoses.Add(jointid, MixedRealityPose.ZeroIdentity);
        }

        _jointPoses = new MixedRealityPose[GestureRecorder._jointIDs.Length];
        referencePose = MixedRealityPose.ZeroIdentity;
        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, m_RuntimeModel);
        inputlabels = m_RuntimeModel.inputs.Select(e => e.ToString()).ToList();
        outputlabels = m_RuntimeModel.outputs;
        dynamicGestureValidator.OrderedStart();
        StartCoroutine(ModelExecutionCoroutine());
    }

    private IEnumerator ModelExecutionCoroutine()
    {
        while (true)
        {
            using (input = new Tensor(1, 1, 9, 1))
            {
                if (!handActive)
                {
                    for (var i = 0; i < outputValues.Length; i++)
                    {
                        outputValues[i] = 0f;
                    }
                    dynamicGestureValidator.PushRemoteDetectionToQueue(outputValues);
                    yield return 0;
                }
                if (!GestureRecorder.TryCalculateJointPoses2(_hand, ref jointPoses))
                {
                    for (var i = 0; i < outputValues.Length; i++)
                    {
                        outputValues[i] = 0f;
                    }
                    dynamicGestureValidator.PushRemoteDetectionToQueue(outputValues);
                    yield return 0;
                }
                GestureRecorder.CalculateJointFeatures(jointPoses, ref features);
                list = features.Values.ToArray();
                for (int i = 0; i < features.Count; i++)
                {
                    input[i] = list[i];
                }
                output = m_Worker.Execute(input).PeekOutput();
                yield return new WaitForCompletion(output);
                z_exp = output.AsFloats().Select(Mathf.Exp).ToArray();
                sum_z_exp = z_exp.Sum();
                softmax = z_exp.Select(i => float.IsNaN(i / sum_z_exp) ? 0f : i / sum_z_exp).ToArray();
                outputValues[0] = softmax[7]; //None
                outputValues[1] = softmax[6]; //Translate
                outputValues[2] = softmax[1]; //Rotate
                outputValues[3] = softmax[2]; //Scale
                outputValues[4] = softmax[0]; //Hover
                outputValues[5] = softmax[3]; //Select
                outputValues[6] = softmax[4]; //TeleportHover
                outputValues[7] = softmax[5]; //TeleportSelect
                PostProcess(ref outputValues);
                dynamicGestureValidator.PushRemoteDetectionToQueue(outputValues);
            }
        }
    }
    
    /*private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying&& handActive)
        {
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.IndexTip].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.MiddleTip].Position, jointPoses[TrackedHandJoint.RingTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.RingTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.ThumbTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.RingTip].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.ThumbTip].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
            Gizmos.DrawLine(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.Palm].Position + -(jointPoses[TrackedHandJoint.Palm].Up)*0.2f);
        }
    }*/
    private void PostProcess(ref float[] res)
    {
        //first figure out if the leftInteractor is facing up or downwards in relation to the world, decides of hover or teleport hover
        bool palmFacingDown = Math.Abs(math.sign(math.dot(Vector3.down, -jointPoses[TrackedHandJoint.Palm].Up)) - 1f) < 0.001f;
        if (palmFacingDown)
        {
            res[4] += res[6];
            res[5] += res[7];
            res[6] = 0f;
            res[7] = 0f;
        }
        else
        {
            res[6] += res[4];
            res[7] += res[5];
            res[4] = 0f;
            res[5] = 0f;
        }
        float indexFacingUpAmount = math.clamp(math.dot(Vector3.up, (jointPoses[TrackedHandJoint.IndexTip].Position - jointPoses[TrackedHandJoint.IndexKnuckle].Position).normalized) + 0.2f,0f,1f);
        var difference = res[2] - res[2] * indexFacingUpAmount;
        res[0] += difference;
        res[2] -= difference;
        res[3] -= difference;
    }
    private void OnDestroy()
    {
        m_Worker?.Dispose();
        input?.Dispose();
        output?.Dispose();
    }
    private void OnEnable()
    {
        handObject.RegisterListener(OnHandSpawned,OnHandDestroyed);
    }
    private void OnHandSpawned(Object obj)
    {
        handActive = true;
    }
    private void OnHandDestroyed(Object obj)
    {
        handActive = false;
    }
    private void OnDisable()
    {
        handObject.UnregisterListener(OnHandSpawned,OnHandDestroyed);
    }
}