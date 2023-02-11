using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class GestureRecorderEvents : ScriptableObject
{
    public List<Gesture> gestures;
    private int _gestureIndex = 0;
    private Action<object> onGestureSelected;
    private bool hasOnGestureSelectedInit;
    public event Action<object> OnGestureSelected
    {
        add
        {
            value.Invoke(gestures[_gestureIndex]);
            onGestureSelected += value;
        }
        remove => onGestureSelected -= value;
    }

    public event Action OnStartRecordingUserGesture;
    public event Action OnDoneRecordingUserGesture;

    public event Action OnStartRecordingDemonstrationGesture;
    public event Action OnDoneRecordingDemonstrationGesture;
    

    public event Action OnStartReplayRecordedUserGesture;
    public event Action OnDoneReplayRecordedUserGesture;

    
    
    [Serializable]
    public enum GestureNavigation
    {
        Back = 0,
        Forward = 1,
        Beginning = 2,
        End = 3
    }
    public void GestureSelected(GestureNavigation gestureNavigation)
    {
        switch (gestureNavigation)
        {
            case GestureNavigation.Back:
                if (_gestureIndex == 0) return;
                _gestureIndex--;
                break;
            case GestureNavigation.Forward:
                if (_gestureIndex == gestures.Count - 1) return;
                _gestureIndex++;
                break;
            case GestureNavigation.Beginning:
                _gestureIndex = 0;
                break;
            case GestureNavigation.End:
                _gestureIndex = gestures.Count - 1;
                break;
        }
        onGestureSelected?.Invoke(gestures[_gestureIndex]);
    }

    public void StartRecordingUserGesture() => OnStartRecordingUserGesture?.Invoke();
    public void DoneRecordingUserGesture() => OnDoneRecordingUserGesture?.Invoke();

    public void StartRecordingDemonstrationGesture() => OnStartRecordingDemonstrationGesture?.Invoke();
    public void DoneRecordingDemonstrationGesture() => OnDoneRecordingDemonstrationGesture?.Invoke();

    public void StartReplayRecordedUserGesture() => OnStartReplayRecordedUserGesture?.Invoke();
    public void DoneReplayRecordedUserGesture() => OnStartReplayRecordedUserGesture?.Invoke();
}
