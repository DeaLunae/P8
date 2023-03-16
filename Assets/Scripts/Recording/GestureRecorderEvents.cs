using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Variables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class GestureRecorderEvents : ScriptableObject
{
    [SerializeField] private GestureList gestures;
    [SerializeField] private IntVariable framesToRecord;
    public int GetFramesToRecord()
    {
        return framesToRecord.Value;
    }
    public int _poseIndex = 0;
    public int NumberOfPoses => gestures.list.Count;
    private Action<object> onGestureSelected;
    private bool hasOnGestureSelectedInit;
    public event Action<object> OnGestureSelected
    {
        add
        {
            value.Invoke(gestures.list[_poseIndex]);
            onGestureSelected += value;
        }
        remove => onGestureSelected -= value;
    }

    public Gesture CurrentGesture => gestures.list[_poseIndex];

    public event Action OnStartRecordingUserGesture;
    public event Action OnDoneRecordingUserGesture;

    public event Action OnStartRecordingDemonstrationGesture;
    public event Action OnDoneRecordingDemonstrationGesture;

    public event Action OnStartReplayRecordedUserGesture;
    public event Action OnDoneReplayRecordedUserGesture;

    public event Action OnStartReplayDemonstrationGesture;
    public event Action OnDoneReplayDemonstrationGesture;
    
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
                if (_poseIndex == 0) return;
                _poseIndex--;
                break;
            case GestureNavigation.Forward:
                if (_poseIndex == gestures.list.Count - 1) return;
                _poseIndex++;
                break;
            case GestureNavigation.Beginning:
                _poseIndex = 0;
                break;
            case GestureNavigation.End:
                _poseIndex = gestures.list.Count - 1;
                break;
        }
        onGestureSelected?.Invoke(gestures.list[_poseIndex]);
    }

    public void StartRecordingUserGesture() => OnStartRecordingUserGesture?.Invoke();
    public void DoneRecordingUserGesture() => OnDoneRecordingUserGesture?.Invoke();

    public void StartRecordingDemonstrationGesture() => OnStartRecordingDemonstrationGesture?.Invoke();
    public void DoneRecordingDemonstrationGesture() => OnDoneRecordingDemonstrationGesture?.Invoke();
    public void StartReplayDemonstrationGesture() => OnStartReplayDemonstrationGesture?.Invoke();
    public void DoneReplayDemonstrationGesture() => OnDoneReplayDemonstrationGesture?.Invoke();
    public void StartReplayRecordedUserGesture() => OnStartReplayRecordedUserGesture?.Invoke();
    public void DoneReplayRecordedUserGesture() => OnDoneReplayRecordedUserGesture?.Invoke();
}
