using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Serialization;

public class ToggleVisibleOnGestureEvent : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Gesture.PoseDataType typeToRender;
    [SerializeField] private List<Renderer> _renderers = new List<Renderer>();

    [SerializeField] private Handedness _handedness;
    private bool correctHandSelected;
    private void GestureSelected(object obj)
    {
        var gesture = (Gesture)obj;
        if (typeToRender == Gesture.PoseDataType.User)
        {
            correctHandSelected = _handedness == gesture.Handedness;
            return;
        }
        _renderers.ForEach(e => e.enabled = _handedness == gesture.Handedness);
    }
    
    void Start()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected += GestureSelected;
        if (typeToRender == Gesture.PoseDataType.User)
        {
            gestureRecorderEvents.OnStartReplayRecordedUserGesture += ToggleVisible;
            gestureRecorderEvents.OnStopReplayRecordedUserGesture += ToggleInvisible;
            _renderers.ForEach(e => e.enabled = false);
        }
    }

    private void ToggleVisible()
    {
        _renderers.ForEach(e => e.enabled = correctHandSelected);
    }
    private void ToggleInvisible()
    {
        _renderers.ForEach(e => e.enabled = false);
    }

    private void OnDestroy()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected -= GestureSelected;
        if (typeToRender == Gesture.PoseDataType.User)
        {
            gestureRecorderEvents.OnStartReplayRecordedUserGesture -= ToggleVisible;
            gestureRecorderEvents.OnStopReplayRecordedUserGesture -= ToggleInvisible;
        }
    }
}
