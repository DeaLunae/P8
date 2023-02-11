using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ToggleVisibleOnGestureEvent : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Handedness _handedness;
    private void GestureSelected(object obj)
    {
        var gesture = (Gesture)obj;
        renderer.enabled = _handedness == gesture.handedness;
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
