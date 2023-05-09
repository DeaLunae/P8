using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using Devkit.Modularis.Channels;
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
    [SerializeField] private TransformChannel leftHand, rightHand;
    public TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.Wrist;
    public static readonly TrackedHandJoint[] _jointIDs =
        ((TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint))).Skip(1).ToArray();
    public bool DebugMode;
    public int secondsToWait;
    private bool DoneRecording => Time.time - timeOffsetTimestamp >= 2f;
    private bool hasBeenSetUp;
    private Dictionary<TrackedHandJoint, MixedRealityPose> jointPoseLookup = new();
    private Dictionary<string, object> outputData = new();
    private Handedness recordingHand = Handedness.None;
    private bool isRecording, leftRecording, rightRecording;
    private LoggingManager _loggingManager;
    private int _frameCountOffset;
    private Stopwatch _timeOffset;
    private float timeOffsetTimestamp;
    private MixedRealityPose[] jointPoses;
    private void Start()
    {
        foreach (var jointid in _jointIDs)
        {
            jointPoseLookup.Add(jointid, MixedRealityPose.ZeroIdentity);
            outputData.Add($"{jointid}.pos.x", float.NaN);
            outputData.Add($"{jointid}.pos.y", float.NaN);
            outputData.Add($"{jointid}.pos.z", float.NaN);
            
            outputData.Add($"{jointid}.rot.x", float.NaN);
            outputData.Add($"{jointid}.rot.y", float.NaN);
            outputData.Add($"{jointid}.rot.z", float.NaN);
            outputData.Add($"{jointid}.rot.w", float.NaN);
            
        }
        outputData.Add("Head.pos.x",float.NaN);
        outputData.Add("Head.pos.y",float.NaN);
        outputData.Add("Head.pos.z",float.NaN);

        outputData.Add("Head.rot.x",float.NaN);
        outputData.Add("Head.rot.y",float.NaN);
        outputData.Add("Head.rot.z",float.NaN);
        outputData.Add("Head.rot.w",float.NaN);
        outputData.Add("Timestamp", float.NaN);
        outputData.Add("FrameCount", float.NaN);
        outputData.Add(RealTimeStamp, float.NaN);
        _loggingManager = FindObjectOfType<LoggingManager>();
        _loggingManager.SetEmail("NA");
        
        jointPoses = new MixedRealityPose[_jointIDs.Length];
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected += GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture += StartRecordingUserGesture;
        }
        Application.wantsToQuit += SaveAllData;
    }


    private DirectoryInfo GetOrCreateDirectory(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        return directoryInfo;
    }

    private bool savingHasBegun = false;
    public bool SaveAllData()
    {
        if (savingHasBegun)
        {
            return false;
        }
        savingHasBegun = true;
        string root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P8GestureData";
        
        GetOrCreateDirectory($"{root}");
        if (DebugMode)
        {
            GetOrCreateDirectory($"{root}\\Debug");
            _loggingManager.SetSavePath($"{root}\\Debug");
        }
        else
        {
            var currLargSubDirNr = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name, out _))
                .Select(e => int.Parse(e.Name)).OrderByDescending(e => e).ToArray();
            DirectoryInfo partDirInf = GetOrCreateDirectory(currLargSubDirNr.Length == 0 ? $"{root}\\{0}" : $"{root}\\{int.Parse(currLargSubDirNr.First().ToString())+1}");
            _loggingManager.SetSavePath(partDirInf.FullName);
        }
        _loggingManager.SaveAllLogs(true,TargetType.CSV);
        Invoke(nameof(DelayedApplicationQuit), secondsToWait);
        return false;
    }


    private void OnDestroy()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture -= StartRecordingUserGesture;
        }
        Application.wantsToQuit -= SaveAllData;
    }

    private void GestureSelected(object o)
    {
        currentGesture = (Gesture)o;
    }
    public string GenerateLogName() => $"gesture-{currentGesture.Name.ToString()}-{(currentGesture.staticGesture ? "Static" : "Dynamic")}-{currentGesture.Handedness.ToString()}-{_gestureRecorderEvents._poseIndex.ToString()}";

    public void StartRecordingUserGesture()
    {
        HandJointUtils.TryGetJointPose(ReferenceJoint, currentGesture.Handedness, out _);
        recordingHand = currentGesture.Handedness;
        _frameCountOffset = Time.frameCount;
        timeOffsetTimestamp = Time.time;
        _timeOffset = Stopwatch.StartNew();
        var logstring = GenerateLogName();
        
        if (_loggingManager.HasLog(logstring))
        {
            _loggingManager.DeleteLog(logstring);
        }
        _loggingManager.CreateLog(logstring);
        
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
        Record();
    }
    public void Record()
    {
        if (!TryCalculateJointPoses(recordingHand, ref jointPoseLookup)) { return; }
        GenerateDataSet(jointPoseLookup, ref outputData);
        _loggingManager.Log(GenerateLogName(), outputData);
    }
    
    
    
    private static List<TrackedHandJoint> keys = new();
    public static bool TryCalculateJointPoses(Handedness handedness, ref Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
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
    private Dictionary<string, float> features = new() {
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
    
    const string HeadRotX = "Head.rot.x";
    const string HeadRotY = "Head.rot.y";
    const string HeadRotZ = "Head.rot.z";
    const string HeadRotW = "Head.rot.w";
    
    const string HeadPosX = "Head.pos.x";
    const string HeadPosY = "Head.pos.y";
    const string HeadPosZ = "Head.pos.z";
    const string Timestamp = "Timestamp";
    const string RealTimeStamp = "RealTimeStamp";
    const string FrameCount = "FrameCount";
    
    private void GenerateDataSet(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses, ref Dictionary<string, object> output)
    {
        foreach (var (joint, pose) in jointPoses)
        {
            var jointStr = joint.ToString();
            output[$"{jointStr}.pos.x"] =  pose.Position.x;
            output[$"{jointStr}.pos.y"] =  pose.Position.y;
            output[$"{jointStr}.pos.z"] =  pose.Position.z;
            
            output[$"{jointStr}.rot.x"] =  pose.Rotation.x;
            output[$"{jointStr}.rot.y"] =  pose.Rotation.y;
            output[$"{jointStr}.rot.z"] =  pose.Rotation.z;
            output[$"{jointStr}.rot.w"] =  pose.Rotation.w;
        }

        var headpos = CameraCache.Main.transform.position;
        var headrot = CameraCache.Main.transform.rotation;
        output[HeadPosX] =  headpos.x;
        output[HeadPosY] =  headpos.y;
        output[HeadPosZ] =  headpos.z;
        
        output[HeadRotX] =  headrot.x;
        output[HeadRotY] =  headrot.y;
        output[HeadRotZ] =  headrot.z;
        output[HeadRotW] =  headrot.w;
        output[Timestamp]  = _timeOffset.ElapsedMilliseconds;
        output[FrameCount] = Time.frameCount - _frameCountOffset;
        output[RealTimeStamp] = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffff");
    }
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
    private void DelayedApplicationQuit()
    {
        Application.wantsToQuit -= SaveAllData;
        Application.Quit();
    }
    public Dictionary<string, object> GenerateLog(Dictionary<string,float> feats)
    {
        var output = new Dictionary<string, object>();

        foreach (var feature in feats)
        {
            output.Add(feature.Key,feature.Value);
        }
        return output;
    }
}