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
    private bool wantsToStop;
    private async void StartRecordingUserIndicator()
    {
        var startFrame = Time.frameCount;
        var start = Time.time;
        await indicator.OpenAsync();
        float progress = 0;
        while (progress < 1)
        {
            progress = (Time.time - start) / 2f;
            indicator.Message = "Optager Tegn...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        await indicator.CloseAsync();
    }

    private async void StartReplayIndicator()
    {
        return;
        var startFrame = Time.frameCount;
        var start = Time.time;
        await indicator.OpenAsync();
        float progress = 0;
        while (!wantsToStop)
        {
            progress = (Time.time - start) / (float) (currentGesture.GetTotalTime(Gesture.PoseDataType.Demonstration)/1000) % 1f;
            indicator.Message = "Afspiller din optagelse...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        wantsToStop = false;
        await indicator.CloseAsync();
    }
    private async void StartDemonstrationIndicator()
    {
        var startFrame = Time.frameCount;
        await indicator.OpenAsync();
        float progress = 0;
        while (progress < 1)
        {
            progress = (Time.frameCount - startFrame) / (float) currentGesture.FrameCount(Gesture.PoseDataType.Demonstration);
            indicator.Message = "Demonstrerer Tegn...";
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
            _gestureRecorderEvents.OnStopReplayRecordedUserGesture += WantsToStop;
        }

        Invoke(nameof(DelayedDeactivate),1);
    }

    private void WantsToStop()
    {
        wantsToStop = true;
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
        _gestureRecorderEvents.OnStopReplayRecordedUserGesture -= WantsToStop;

    }
}
