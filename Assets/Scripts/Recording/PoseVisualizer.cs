using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

public class PoseVisualizer : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Transform _anchorTransform;
    private GameObject spawnedPose;
    private Gesture currentGesture;
    private Vector3 GetCenterPosition(IReadOnlyCollection<Transform> transforms)
    {
        return transforms.Aggregate(Vector3.zero, (curr, t) => curr + t.position) / transforms.Count;
    }
    
    private void GestureSelected(object obj)
    {
        currentGesture = (Gesture)obj;
    }
    private void OnEnable()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected += GestureSelected;
    }
    private void OnDisable()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected -= GestureSelected;
    }
}
