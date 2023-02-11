using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Dwell;
using Microsoft.MixedReality.Toolkit.UI;
using OculusSampleFramework;
using UnityEngine;
using Interactable = Microsoft.MixedReality.Toolkit.UI.Interactable;

public class DeselectOnGestureEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private PressableButtonHoloLens2 _pressableButton;
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private bool _onDoneReplaying;
    [SerializeField] private bool _onDoneRecording;


    private void DeselectRecordButton()
    {
        if (_pressableButton == null || !_onDoneRecording) return;
        GetComponent<Interactable>().CanDeselect = true;
        
        _pressableButton.ButtonReleased.Invoke();
        print("aa");
    }
    private void DeselectReplayButton()
    {
        if (_pressableButton == null || !_onDoneReplaying) return;
        GetComponent<Interactable>().CanDeselect = true;
        _pressableButton.ButtonReleased.Invoke();
        print("aaa");

    }
    
    void OnEnable()
    {
        _pressableButton = GetComponent<PressableButtonHoloLens2>();
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnDoneReplayRecordedUserGesture += DeselectReplayButton;
        gestureRecorderEvents.OnDoneRecordingUserGesture += DeselectRecordButton;
    }
    private void OnDisable()
    {
        if (gestureRecorderEvents == null) return;
        gestureRecorderEvents.OnDoneReplayRecordedUserGesture -= DeselectReplayButton;
        gestureRecorderEvents.OnDoneRecordingUserGesture -= DeselectRecordButton;
    }
}
