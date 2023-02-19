using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

[Serializable]
public class Gesture
{
    [SerializeField] private GestureName gestureName;
    [SerializeField] private GestureType gestureType;
    [SerializeField] private Handedness handedness;
    [SerializeField] private List<PoseFrameData> demonstrationData = new ();
    private List<PoseFrameData> _userData = new ();
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
                return _userData.Count;
        }
        return -1;
    }

    public float GetTotalTime(PoseDataType dataType)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                if (demonstrationData.Count == 0)
                {
                    break;
                }
                return demonstrationData.LastOrDefault().time;
            case PoseDataType.User:
                if (_userData.Count == 0)
                {
                    break;
                }
                return _userData.LastOrDefault().time;
        }
        return 1f;
    }
    public void ResetPoseData(PoseDataType dataType)
    {
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                demonstrationData.Clear();
                break;
            case PoseDataType.User:
                _userData.Clear();
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
                return _userData[i];
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
                correctDataset = _userData;
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
        poseFrameData.positions[0] = Vector3.zero;
        switch (dataType)
        {
            case PoseDataType.Demonstration:
                demonstrationData.Insert(0,poseFrameData);
                break;
            case PoseDataType.User:
                _userData.Add(poseFrameData);
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
            poseFrameData.positions = new List<Vector3>();
            poseFrameData.rotations = new List<Quaternion>();
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
        None,
        A, 
        B, 
        C, 
        D, 
        E, 
        F, 
        G, 
        H, 
        I, 
        J, 
        K, 
        L, 
        M, 
        N, 
        O, 
        P, 
        Q, 
        R, 
        S, 
        T, 
        U, 
        V, 
        W, 
        X, 
        Y, 
        Z
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
