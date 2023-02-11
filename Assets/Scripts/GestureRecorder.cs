using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GestureRecorder : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    private Gesture currentGesture;
    private bool DoneRecording => Time.frameCount - _frameCountOffset == currentGesture.amountOfSampleFrames - 1;
    public TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.Wrist;
    private Handedness recordingHand = Handedness.None;
    private bool isRecording = false, leftRecording = false, rightRecording = false;
    private LoggingManager _loggingManager;
    private int _frameCountOffset;
    private Stopwatch _timeOffset;
    private MixedRealityPose[] jointPoses;

    private string root =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P7GestureData";
    public static readonly TrackedHandJoint[] _jointIDs =
    {
        TrackedHandJoint.Wrist,
        TrackedHandJoint.Palm,
        TrackedHandJoint.ThumbProximalJoint,
        TrackedHandJoint.ThumbDistalJoint,
        TrackedHandJoint.ThumbTip,
        TrackedHandJoint.IndexKnuckle,
        TrackedHandJoint.IndexMiddleJoint,
        TrackedHandJoint.IndexDistalJoint,
        TrackedHandJoint.IndexTip,
        TrackedHandJoint.MiddleKnuckle,
        TrackedHandJoint.MiddleMiddleJoint,
        TrackedHandJoint.MiddleDistalJoint,
        TrackedHandJoint.MiddleTip,
        TrackedHandJoint.RingKnuckle,
        TrackedHandJoint.RingMiddleJoint,
        TrackedHandJoint.RingDistalJoint,
        TrackedHandJoint.RingTip,
        TrackedHandJoint.PinkyKnuckle,
        TrackedHandJoint.PinkyMiddleJoint,
        TrackedHandJoint.PinkyDistalJoint,
        TrackedHandJoint.PinkyTip
    };

    private Dictionary<TrackedHandJoint, MixedRealityPose> jointPoseLookup = new();

    public bool DebugMode;

    private void Start()
    {
        foreach (var jointid in _jointIDs)
        {
            jointPoseLookup.Add(jointid, MixedRealityPose.ZeroIdentity);
        }
        _loggingManager = FindObjectOfType<LoggingManager>();
        _loggingManager.SetEmail("NA");
        
        jointPoses = new MixedRealityPose[_jointIDs.Length];
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected += GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture += StartRecordingUserGesture;
            
        }
    }

    private void OnApplicationQuit()
    {
        if (DebugMode)
        {
            var directoryInfo = new DirectoryInfo($"{root}\\Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _loggingManager.SetSavePath($"{root}\\Debug");
        }
        else
        {
            var currentLargestSubDirectoryNumber = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name,out _)).OrderByDescending(e => e.Name).ToArray();
            DirectoryInfo dirinfo;
            if (currentLargestSubDirectoryNumber.Length == 0)
            {
                 dirinfo = new DirectoryInfo($"{root}\\{0}");
            }
            else
            {
                dirinfo = new DirectoryInfo($"{root}\\{int.Parse(currentLargestSubDirectoryNumber[0].Name)+1}");
            }
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }
            _loggingManager.SetSavePath(dirinfo.FullName);
        }
        _loggingManager.SaveAllLogs(true);
    }

    private void OnDestroy()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture -= StartRecordingUserGesture;
        }
    }

    private void GestureSelected(object o)
    {
        currentGesture = (Gesture)o;
    }
    
    public void StartRecordingUserGesture()
    {
        HandJointUtils.TryGetJointPose(ReferenceJoint, currentGesture.handedness, out MixedRealityPose joint);
        
        recordingHand = currentGesture.handedness;
        _frameCountOffset = Time.frameCount;
        _timeOffset = Stopwatch.StartNew();
        
        if (_loggingManager.HasLog($"gesture-{currentGesture.id.ToString()}-{currentGesture.handedness.ToString()}"))
        {
            _loggingManager.DeleteLog($"gesture-{currentGesture.id.ToString()}-{currentGesture.handedness.ToString()}");
        }
        _loggingManager.CreateLog($"gesture-{currentGesture.id.ToString()}-{currentGesture.handedness.ToString()}");
        
        isRecording = true;
    }
    public void StopRecording()
    {
        _timeOffset.Reset();
        isRecording = false;
        _gestureRecorderEvents.DoneRecordingUserGesture();
    }

    private void Update()
    {
        if (!isRecording) return;
        if (DoneRecording)
        {
            StopRecording();
            return;
        }
        RecordFrame2();
    }
    
    private MixedRealityPose _referencePose = MixedRealityPose.ZeroIdentity;
    public void RecordFrame()
    {
        _referencePose = MixedRealityPose.ZeroIdentity;
        if (!HandJointUtils.TryGetJointPose(ReferenceJoint, recordingHand, out _referencePose)) return;
        if (!TryCalculateJointPoses(
                _referencePose.Rotation, 
                _referencePose.Position,
                recordingHand, 
                _jointIDs,
                ref jointPoses))
        {
            return;
        }
        var log = GenerateLog();
        if (log == null)
        {
            return;
        }
        _loggingManager.Log($"gesture-{currentGesture.id.ToString()}-{currentGesture.handedness.ToString()}", log);
    }
    
    public void RecordFrame2()
    {
        if (!TryCalculateJointPoses2(recordingHand, ref jointPoseLookup))
        {
            return;
        }

        CalculateJointFeatures(jointPoseLookup, ref features);
        
        var log = GenerateLog2(features);
        if (log == null)
        {
            return;
        }
        _loggingManager.Log($"gesture-{currentGesture.id.ToString()}-{currentGesture.handedness.ToString()}", log);
    }
    public static bool TryCalculateJointPoses(Quaternion offsetRotation, Vector3 offsetPosition, Handedness handedness,TrackedHandJoint[] joints, ref MixedRealityPose[] poses)
    {
        Quaternion invRotation = Quaternion.Inverse(offsetRotation);
        Quaternion invCameraRotation = Quaternion.Inverse(CameraCache.Main.transform.rotation);
        //var cameraOffsetPos = CameraCache.Main.transform.position;
        for (int i = 0; i < poses.Length; i++)
        {
            poses[i] = MixedRealityPose.ZeroIdentity;
        }
        for (int i = 0; i < poses.Length; i++)
        {
            if (!HandJointUtils.TryGetJointPose(joints[i], handedness, out poses[i]))
            {
                return false;
            }
            // Apply inverse external transform
            poses[i].Position = invRotation * (poses[i].Position - offsetPosition);
            poses[i].Rotation = invRotation * poses[i].Rotation;

            // To camera space
            poses[i].Position = invCameraRotation * poses[i].Position;
            poses[i].Rotation = invCameraRotation * poses[i].Rotation;
            //removing camera offsets as well.
        }
        return true;
    }

    private static List<TrackedHandJoint> keys = new List<TrackedHandJoint>();
    public static bool TryCalculateJointPoses2(Handedness handedness, ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
    {
        keys.Clear();
        keys.AddRange(jointPoses.Keys);
        foreach (var key in keys)
        {
            jointPoses[key] = MixedRealityPose.ZeroIdentity;
            if (!HandJointUtils.TryGetJointPose(key, handedness, out var pose)) { return false; }
            jointPoses[key] = pose;
        }
        return true;
    }

    public Dictionary<string, object> GenerateLog()
    {
        var output = new Dictionary<string, object>();
        output.Add("Handedness", recordingHand.ToString());
        output.Add("Timestamp", _timeOffset.ElapsedMilliseconds.ToString());
        output.Add("Framecount", Time.frameCount - _frameCountOffset);
        output.Add("SessionID", "NA");
        output.Add("Email", "NA");
        for (var i = 0; i < jointPoses.Length; ++i)
        {                                                      
            var itemString = _jointIDs[i].ToString();
            
            output.Add($"{itemString}PosX",jointPoses[i].Position.x);
            output.Add($"{itemString}PosY",jointPoses[i].Position.y);
            output.Add($"{itemString}PosZ",jointPoses[i].Position.z);
            
            output.Add($"{itemString}RotX",jointPoses[i].Rotation.x);
            output.Add($"{itemString}RotY",jointPoses[i].Rotation.y);
            output.Add($"{itemString}RotZ",jointPoses[i].Rotation.z);
            output.Add($"{itemString}RotW",jointPoses[i].Rotation.w);

        }
        return output;
    }

    public const string D_Palm_ThumbTip = "D_Palm_ThumbTip";
    public const string D_Palm_IndexTip = "D_Palm_IndexTip";
    public const string D_Palm_MiddleTip = "D_Palm_MiddleTip";
    public const string D_Palm_RingTip = "D_Palm_RingTip";
    public const string D_Palm_PinkyTip = "D_Palm_PinkyTip";
    public const string D_Thumbtip_Indextip = "D_Thumbtip_Indextip";
    public const string D_IndexTip_MiddleTip = "D_IndexTip_MiddleTip";
    public const string D_MiddleTip_RingTip = "D_MiddleTip_RingTip";
    public const string D_RingTip_PinkyTip = "D_RingTip_PinkyTip";
    public const string Palm_Rot_X = "Palm_Rot_X";
    public const string Palm_Rot_Y = "Palm_Rot_Y";
    public const string Palm_Rot_Z = "Palm_Rot_Z";
    public const string Palm_Rot_W = "Palm_Rot_W";
    private Dictionary<string, float> features = new()
    {
        {D_IndexTip_MiddleTip,0},
        {D_MiddleTip_RingTip,0},
        {D_Palm_IndexTip,0},
        {D_Palm_MiddleTip,0},
        {D_Palm_PinkyTip,0},
        {D_Palm_RingTip,0},
        {D_Palm_ThumbTip,0},
        {D_RingTip_PinkyTip,0},
        {D_Thumbtip_Indextip,0},
        /*{Palm_Rot_W,0},
        {Palm_Rot_X,0},
        {Palm_Rot_Y,0},
        {Palm_Rot_Z,0}*/
    };
    
    public static void CalculateJointFeatures(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses, ref Dictionary<string, float> features)
    {
        //D_Palm_ThumbTip
        features[D_Palm_ThumbTip] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.ThumbTip].Position);
        //D_Palm_IndexTip
        features[D_Palm_IndexTip] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
        //D_Palm_MiddleTip
        features[D_Palm_MiddleTip] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
        //D_Palm_RingTip
        features[D_Palm_RingTip] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.RingTip].Position);
        //D_Palm_PinkyTip
        features[D_Palm_PinkyTip] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);

        //D_Thumbtip_Indextip
        features[D_Thumbtip_Indextip] = Vector3.Distance(jointPoses[TrackedHandJoint.ThumbTip].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
        //D_IndexTip_MiddleTip
        features[D_IndexTip_MiddleTip] = Vector3.Distance(jointPoses[TrackedHandJoint.IndexTip].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
        //D_MiddleTip_RingTip
        features[D_MiddleTip_RingTip] = Vector3.Distance(jointPoses[TrackedHandJoint.MiddleTip].Position, jointPoses[TrackedHandJoint.RingTip].Position);
        //D_RingTip_PinkyTip
        features[D_RingTip_PinkyTip] = Vector3.Distance(jointPoses[TrackedHandJoint.RingTip].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);
        
        /*var cameraFlat = CameraCache.Main.transform.forward;
        cameraFlat.y = 0;
        var hostForwardFlat = jointPoses[TrackedHandJoint.Palm].Forward;
        hostForwardFlat.y = 0;
        var hostRotFlat = Quaternion.LookRotation(hostForwardFlat, Vector3.up);
        var inverseRotation = Quaternion.Inverse(Quaternion.LookRotation(cameraFlat, Vector3.up)) * hostRotFlat;
        //Palm_Rot_X
        features[Palm_Rot_X] = inverseRotation.x;
        //Palm_Rot_Y
        features[Palm_Rot_Y] = inverseRotation.y;
        //Palm_Rot_Z
        features[Palm_Rot_Z] = inverseRotation.z;
        //Palm_Rot_Y
        features[Palm_Rot_W] = inverseRotation.w;*/
    }
    private void OnDrawGizmos()
    {
        /*if (Application.isPlaying && isRecording)
        {
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.IndexTip].Position, jointPoseLookup[TrackedHandJoint.MiddleTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.MiddleTip].Position, jointPoseLookup[TrackedHandJoint.RingTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.IndexTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.MiddleTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.PinkyTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.RingTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.ThumbTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.RingTip].Position, jointPoseLookup[TrackedHandJoint.PinkyTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.ThumbTip].Position, jointPoseLookup[TrackedHandJoint.IndexTip].Position);
            Gizmos.DrawLine(jointPoseLookup[TrackedHandJoint.Palm].Position, jointPoseLookup[TrackedHandJoint.Palm].Position + -(jointPoseLookup[TrackedHandJoint.Palm].Up)*0.2f);
        }*/
    }
    public Dictionary<string, object> GenerateLog2(Dictionary<string,float> feats)
    {
        var output = new Dictionary<string, object>();
        output.Add("Handedness", recordingHand.ToString());
        output.Add("Timestamp", _timeOffset.ElapsedMilliseconds.ToString());
        output.Add("Framecount", Time.frameCount - _frameCountOffset);
        output.Add("SessionID", "NA");
        output.Add("Email", "NA");
        foreach (var feature in feats)
        {
            output.Add(feature.Key,feature.Value);
        }
        return output;
    }
    

}