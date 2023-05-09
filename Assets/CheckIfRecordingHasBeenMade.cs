using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using MRKTExtensions.Ux;
using UnityEngine;

public class CheckIfRecordingHasBeenMade : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;

    private void Start()
    {
        _gestureRecorderEvents.OnGestureSelected += CheckIfRecordingHasBeenMadeOnOnGestureSelected;
    }

    private void CheckIfRecordingHasBeenMadeOnOnGestureSelected(object obj)
    {
        if (_gestureRecorderEvents.CurrentGesture.UserData == null)
        {
            DisableButton();
            return;
        }
        if (_gestureRecorderEvents.CurrentGesture.UserData.Count != 0) return;
        DisableButton();
    }

    private void DisableButton()
    {
        GetComponent<ButtonStatusController>().SetStatus(false);
    }
}
