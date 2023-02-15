using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ReplayDemonstrationTransforms : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Transform handRoot;
    [SerializeField] private Handedness hand;
    private Transform[] _handRootTransforms;
    private int _currentIndex;
    private bool _playing;
    private Gesture.PoseFrameData _currentPoseFrameData;
    private Stopwatch _stopwatch;
    private int _stopTime;

    private void Update()
    {
        if (_playing)
        {
            PlayNextTransformPose();
        }
    }

    private void PlayNextTransformPose()
    {
        if (_stopwatch.ElapsedMilliseconds >= _stopTime)
        {
            //We're doing replaying, so stop the replay here
            gestureRecorderEvents.DoneReplayDemonstrationGesture();
            _playing = false;
            return;
        }
        _currentPoseFrameData = gestureRecorderEvents.CurrentGesture.GetPoseAtTime(Gesture.PoseDataType.Demonstration, _stopwatch.ElapsedMilliseconds);
        for (var i = 0; i < _handRootTransforms.Length; i++)
        {
            _handRootTransforms[i].SetPositionAndRotation(
                _currentPoseFrameData.positions[i],
                _currentPoseFrameData.rotations[i]);
        }
    }

    private void StartReplayTransforms()
    {
        handRoot.gameObject.SetActive(true);
        _playing = true;
        _currentIndex = 0;
        _stopwatch = Stopwatch.StartNew();
        _stopTime = gestureRecorderEvents.CurrentGesture.FrameCount(Gesture.PoseDataType.Demonstration);
    }
    private void OnEnable()
    {
        gestureRecorderEvents.OnGestureSelected += SubscribeIfCorrectHand;
    }
    private void SubscribeIfCorrectHand(object obj)
    {
        gestureRecorderEvents.OnStartReplayDemonstrationGesture -= StartReplayTransforms;
        if (gestureRecorderEvents.CurrentGesture.Handedness != hand) { return; }
        gestureRecorderEvents.OnStartReplayDemonstrationGesture += StartReplayTransforms;
    }
    private void OnDisable()
    {
        gestureRecorderEvents.OnStartReplayDemonstrationGesture -= StartReplayTransforms;
        gestureRecorderEvents.OnGestureSelected -= SubscribeIfCorrectHand;
    }
}
