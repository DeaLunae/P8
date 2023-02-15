using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.VisualScripting;
using UnityEngine;


public class ProgressIndicatorEvent : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    private ProgressIndicatorLoadingBar indicator;
    private Gesture currentGesture;
    private int currentStartFrame;

    private async void StartRecordingUserIndicator()
    {
        currentStartFrame= Time.frameCount;
        await indicator.OpenAsync();
        float progress = 0;
        while (progress < 1)
        {
            progress = (Time.frameCount - currentStartFrame) / currentGesture.DemonstrationFrameCount();
            indicator.Message = "Recording Pose...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        await indicator.CloseAsync();
    }

    private async void StartReplayIndicator()
    {
        var startFrame = Time.frameCount;
        await indicator.OpenAsync();
        float progress = 0;
        while (progress < 1)
        {
            progress = (Time.frameCount - startFrame) / (float) currentGesture.DemonstrationFrameCount();
            indicator.Message = "Replaying your recording...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        await indicator.CloseAsync();
    }
    private async void StartDemonstrationIndicator()
    {
        var startFrame = Time.frameCount;
        await indicator.OpenAsync();
        float progress = 0;
        while (progress < 1)
        {
            
            progress = (Time.frameCount - startFrame) / (float) currentGesture.DemonstrationFrameCount();
            indicator.Message = "Demonstrating gesture...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        await indicator.CloseAsync();
    }
    private void GestureSelected(object o)
    {
        currentGesture = (Gesture) o;
    }
    private void Start()
    {
        indicator = GetComponent<ProgressIndicatorLoadingBar>();
        indicator.Progress = 0f;
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected += GestureSelected;
            _gestureRecorderEvents.OnStartReplayRecordedUserGesture += StartReplayIndicator;
            _gestureRecorderEvents.OnStartRecordingUserGesture += StartRecordingUserIndicator;
        }

        Invoke(nameof(DelayedDeactivate),1);
    }

    private void DelayedDeactivate()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_gestureRecorderEvents == null) return;
        _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
        _gestureRecorderEvents.OnStartReplayRecordedUserGesture -= StartReplayIndicator;
        _gestureRecorderEvents.OnStartRecordingUserGesture -= StartRecordingUserIndicator;

    }
}
