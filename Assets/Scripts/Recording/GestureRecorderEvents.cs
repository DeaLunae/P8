using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devkit.Modularis.Variables;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class GestureRecorderEvents : ScriptableObject
{
    [SerializeField] private GestureList gestures;
    [SerializeField] private IntVariable framesToRecord;
    [NonSerialized] public List<Gesture> wrongGestures;
    public bool ContainsSpecific(Gesture.GestureName gestureName)
    {
        return wrongGestures.Count(e => e.Name == gestureName) > 0;
    }
   
    public int GetFramesToRecord()
    {
        return framesToRecord.Value;
    }
    public int _poseIndex = 0;
    public int NumberOfPoses => endOfListReached ? wrongGestures.Count : gestures.list.Count;
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

    public Gesture CurrentGesture => endOfListReached ? wrongGestures[_poseIndex] : gestures.list[_poseIndex];
    public List<Gesture> GestureList
    {
        get => gestures.list;
        set => gestures.list = value;
    }

    public event Action OnStartRecordingUserGesture;
    public event Action OnDoneRecordingUserGesture;

    public event Action OnStartRecordingDemonstrationGesture;
    public event Action OnDoneRecordingDemonstrationGesture;

    public event Action OnStartReplayRecordedUserGesture;
    public event Action OnStopReplayRecordedUserGesture;

    public event Action OnDoneReplayRecordedUserGesture;

    public event Action OnStartReplayDemonstrationGesture;
    public event Action OnDoneReplayDemonstrationGesture;
    
    public event Action OnKeyboardButtonPressed;
    public event Action EndOfListReached;
    [NonSerialized] public bool endOfListReached = false;

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
                if (_poseIndex == (endOfListReached ? wrongGestures.Count - 1 : gestures.list.Count - 1))
                {
                    if (!endOfListReached)
                    {
                        wrongGestures = gestures.list.FindAll(gesture => gesture.UserSessionResponse != gesture.Name);
                        endOfListReached = true;
                        _poseIndex = 0;
                        EndOfListReached?.Invoke();
                    } 
                    break;
                }
                _poseIndex++;
                break;
            case GestureNavigation.Beginning:
                _poseIndex = 0;
                break;
            case GestureNavigation.End:
                _poseIndex = gestures.list.Count - 1;
                break;
        }
        onGestureSelected?.Invoke(endOfListReached ? wrongGestures[_poseIndex] : gestures.list[_poseIndex]);
    }
    
    public void SetUserSessionResponse(Gesture.GestureName gestureName)
    {
        gestures.list[_poseIndex].UserSessionResponse = gestureName;
        OnKeyboardButtonPressed?.Invoke();
    }

    public void StartRecordingUserGesture() => OnStartRecordingUserGesture?.Invoke();
    public void StopReplayRecordedUserGesture() => OnStopReplayRecordedUserGesture?.Invoke();
    public void DoneRecordingUserGesture() => OnDoneRecordingUserGesture?.Invoke();

    public void StartRecordingDemonstrationGesture() => OnStartRecordingDemonstrationGesture?.Invoke();
    public void DoneRecordingDemonstrationGesture() => OnDoneRecordingDemonstrationGesture?.Invoke();
    public void StartReplayDemonstrationGesture() => OnStartReplayDemonstrationGesture?.Invoke();
    public void DoneReplayDemonstrationGesture() => OnDoneReplayDemonstrationGesture?.Invoke();
    public void StartReplayRecordedUserGesture()
    {
        if (CurrentGesture.UserData.Count == 0)
        {
            StopReplayRecordedUserGesture();
            return;
        }
        OnStartReplayRecordedUserGesture?.Invoke();
    }

    public void DoneReplayRecordedUserGesture() => OnDoneReplayRecordedUserGesture?.Invoke();
    public void OnKeyboardButtonPressedEvent() => OnKeyboardButtonPressed?.Invoke();
}
