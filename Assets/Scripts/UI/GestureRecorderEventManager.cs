using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GestureRecorderEventManager : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;

    private void Start()
    {
        if (gestureRecorderEvents != null)
        {
            Invoke(nameof(setupGesture), 1f);
        }
    }

    private void setupGesture()
    {
        gestureRecorderEvents.GestureSelected(GestureRecorderEvents.GestureNavigation.Beginning);
    }
    public void GestureSelected(GestureNavigationEventEnum gestureNavigationEventEnum)
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.GestureSelected(gestureNavigationEventEnum._gestureNavigationVal);
        }
    }

    public void StartRecordingUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("start recording user gesture");
            gestureRecorderEvents.StartRecordingUserGesture();
        }
    }
    
    public void DoneRecordingUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("done recording user gesture");
            gestureRecorderEvents.DoneRecordingUserGesture();
        }
    }

    public void StartReplayRecordedUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("start replay recorded user gesture");
            gestureRecorderEvents.StartReplayRecordedUserGesture();
        }
    }
    
    public void StartReplayDemonstrationGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("start replay demonstration gesture");
            gestureRecorderEvents.StartReplayDemonstrationGesture();
        }
    }
    public void StopReplayRecordedUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("stop replay recorded user gesture");
            gestureRecorderEvents.StopReplayRecordedUserGesture();
        }
    }
    public void DoneReplayRecordedUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("done replay recorded user gesture");
            gestureRecorderEvents.DoneReplayRecordedUserGesture();
        }
    }
    
    public void StartRecordingDemonstrationGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("start recording demonstration gesture");
            gestureRecorderEvents.StartRecordingDemonstrationGesture();
        }
    }
    public void DoneRecordingDemonstrationGesture()
    {
        if (gestureRecorderEvents != null)
        {
            print("done recording demonstration gesture");
            gestureRecorderEvents.DoneRecordingDemonstrationGesture();
        }
    }
}

