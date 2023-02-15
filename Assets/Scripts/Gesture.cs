using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

[CreateAssetMenu]
public class Gesture : ScriptableObject
{
    [SerializeField] private GestureName gestureName;
    
    [SerializeField] private GestureType gestureType;
    [SerializeField] private Handedness handedness;
    [SerializeField] private List<PoseFrameData> demonstrationData = new ();
    private List<PoseFrameData> userData = new ();

    public GestureName Name => gestureName;
    public GestureType Type => gestureType;
    public Handedness Handedness => handedness;

    


    public int FrameCount(PoseDataType dataType)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                return demonstrationData.Count;
            case PoseDataType.User:
                return userData.Count;
        }
        return -1;
    }

    public float GetTotalTime(PoseDataType dataType)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                return demonstrationData.Last().time;
            case PoseDataType.User:
                return userData.Last().time;
        }
        return -1;
    }
    public void ResetPoseData(PoseDataType dataType)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                demonstrationData.Clear();
                break;
            case PoseDataType.User:
                userData.Clear();
                break;
        }
    }

    public PoseFrameData GetPoseFrame(PoseDataType dataType, int i)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                return demonstrationData[i];
            case PoseDataType.User:
                return userData[i];
        }
        return new PoseFrameData();
    }

    public PoseFrameData GetPoseAtTime(PoseDataType dataType, float time)
    {
        List<PoseFrameData> correctDataset = null;
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                correctDataset = demonstrationData;
                break;
            case PoseDataType.User:
                correctDataset = userData;
                break;
        }
        if (time >= GetTotalTime(dataType))
        {
            return correctDataset.Last();
        }
        for (int i = 1; i < correctDataset.Count; i++)
        {
            if (!(correctDataset[i].time > time)) continue;
            return PoseFrameData.Interpolate(
                correctDataset[i - 1],
                correctDataset[i],
                math.remap(
                    correctDataset[i - 1].time,
                    correctDataset[i].time, 
                    0,
                    1,
                    time - correctDataset[i - 1].time));
        }
        return correctDataset.First();
    }
    
    public void AddPoseFrame(PoseDataType dataType, PoseFrameData poseFrameData)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                demonstrationData.Add(poseFrameData);
                break;
            case PoseDataType.User:
                userData.Add(poseFrameData);
                break;
        }
    }
    [Serializable]
    public struct PoseFrameData
    {
        public List<Vector3> positions;
        public List<Quaternion> rotations;
        public float time;

        public static PoseFrameData Interpolate(PoseFrameData from, PoseFrameData to, float value)
        {
            var poseFrameData = new PoseFrameData();
            for (int i = 0; i < from.positions.Count; i++)
            {
                poseFrameData.positions.Add(Vector3.Lerp(from.positions[i],to.positions[i],value));
                poseFrameData.rotations.Add(Quaternion.Lerp(from.rotations[i],to.rotations[i],value));
            }
            poseFrameData.time = from.time + (to.time - from.time) * value;
            return poseFrameData;
        }
    }
    
    [Serializable]
    public enum GestureName
    {
        None
    }
    [Serializable]
    public enum GestureType
    {
        Static,
        Dynamic
    }

    [Serializable]
    public enum PoseDataType
    {
        Demonstration,
        User
    }
}
