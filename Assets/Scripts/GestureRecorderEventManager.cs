using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GestureRecorderEventManager : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    
    // Start is called before the first frame update

    private void Start()
    {
        if (gestureRecorderEvents != null)
        {
            Invoke(nameof(setupGesture), 0.2f);
        }
        
    }

    private void setupGesture()
    {
        gestureRecorderEvents.GestureSelected(GestureRecorderEvents.GestureNavigation.Beginning);

    }
    public void GestureSelected(GestureClickEvent gestureClickEvent)
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.GestureSelected(gestureClickEvent._gestureNavigationVal);
        }
    }

    public void StartRecordingUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.StartRecordingUserGesture();
        }
    }
    
    public void DoneRecordingUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.DoneRecordingUserGesture();
        }
    }

    public void StartReplayRecordedUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.StartReplayRecordedUserGesture();
        }
    }

    public void DoneReplayRecordedUserGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.DoneReplayRecordedUserGesture();
        }
    }
    
    public void StartRecordingDemonstrationGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.StartRecordingDemonstrationGesture();
        }
    }
    public void DoneRecordingDemonstrationGesture()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.DoneRecordingDemonstrationGesture();
        }
    }
}

