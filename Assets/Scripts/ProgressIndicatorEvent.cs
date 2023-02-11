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
            var TotalSamples = (float) currentGesture.amountOfSampleFrames;
            progress = (Time.frameCount - currentStartFrame) / TotalSamples;
            indicator.Message = "Recording...";
            indicator.Progress = progress;
            await Task.Yield();
        }
        
        indicator.Message = $"Current pose: {currentGesture.id.ToString()}";
        await indicator.CloseAsync();
    }

    private async void StartStartReplayIndicator()
    {
        var startFrame = Time.frameCount;
        await indicator.OpenAsync();

        float progress = 0;
        while (progress < 1)
        {
            
            progress = (Time.frameCount - startFrame) / (float) currentGesture.amountOfSampleFrames;
            indicator.Message = "Replaying your recording...";
            indicator.Progress = progress;
            
            await Task.Yield();
        }
        indicator.Message = $"Current pose: {currentGesture.id.ToString()}";

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
            _gestureRecorderEvents.OnStartReplayRecordedUserGesture += StartStartReplayIndicator;
            _gestureRecorderEvents.OnStartRecordingUserGesture += StartRecordingUserIndicator;
        }

        Invoke(nameof(Delayedshutup),1);
        
        
    }

    private void Delayedshutup()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
            _gestureRecorderEvents.OnStartReplayRecordedUserGesture -= StartStartReplayIndicator;
            _gestureRecorderEvents.OnStartRecordingUserGesture -= StartRecordingUserIndicator;
        }
        
    }
}
