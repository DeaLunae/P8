using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

public class ToggleVisibleOnGestureEvent : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Renderer _render;
    [SerializeField] private Handedness _handedness;
    private void GestureSelected(object obj)
    {
        var gesture = (Gesture)obj;
        _render.enabled = _handedness == gesture.Handedness;
    }
    
    void Start()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected += GestureSelected;
    }
    private void OnDestroy()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnGestureSelected -= GestureSelected;
    }
}
