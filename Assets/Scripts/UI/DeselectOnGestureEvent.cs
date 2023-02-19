using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Dwell;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Interactable = Microsoft.MixedReality.Toolkit.UI.Interactable;

public class DeselectOnGestureEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private PressableButtonHoloLens2 _pressableButton;
    private Interactable _interactable;
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private ButtonType buttonType;
    [Serializable]
    public enum ButtonType
    {
        ReplayUser,
        ReplayDemo,
        RecordUser,
        RecordDemo
    }


    private void DeselectButton()
    {
        _interactable.TriggerOnClick(force:true);
    }
    void OnEnable()
    {
        _pressableButton = GetComponent<PressableButtonHoloLens2>();
        _interactable = GetComponent<Interactable>();

        if (gestureRecorderEvents == null) return;
        switch (buttonType)
        {
            case ButtonType.ReplayUser:
                gestureRecorderEvents.OnDoneReplayRecordedUserGesture += DeselectButton;
                break;
            case ButtonType.ReplayDemo:
                gestureRecorderEvents.OnDoneReplayDemonstrationGesture += DeselectButton;
                break;
            case ButtonType.RecordUser:
                gestureRecorderEvents.OnDoneRecordingUserGesture += DeselectButton;
                break;
            case ButtonType.RecordDemo:
                gestureRecorderEvents.OnDoneRecordingDemonstrationGesture += DeselectButton;
                break;
        }
    }
    private void OnDisable()
    {
        if (gestureRecorderEvents == null) return;
        switch (buttonType)
        {
            case ButtonType.ReplayUser:
                gestureRecorderEvents.OnDoneReplayRecordedUserGesture -= DeselectButton;
                break;
            case ButtonType.ReplayDemo:
                gestureRecorderEvents.OnDoneReplayDemonstrationGesture -= DeselectButton;
                break;
            case ButtonType.RecordUser:
                gestureRecorderEvents.OnDoneRecordingUserGesture -= DeselectButton;
                break;
            case ButtonType.RecordDemo:
                gestureRecorderEvents.OnDoneRecordingDemonstrationGesture -= DeselectButton;
                break;
        }
    }
}
