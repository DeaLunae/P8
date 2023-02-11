using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class GestureDetectorEvent : ScriptableObject
{
    public Handedness handedness;
    public Gesture.GestureID gestureID;
    public float probability;
    
}