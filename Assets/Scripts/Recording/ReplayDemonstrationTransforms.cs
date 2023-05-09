using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ReplayDemonstrationTransforms : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Transform leftHand, rightHand;
    [SerializeField] private bool loop;
    
    private Transform[] _leftHandRootTransforms, _rightHandRootTransforms, _currentRootTransforms;
    private int _currentIndex;
    private bool _playing;
    private Gesture.PoseFrameData _currentPoseFrameData;
    private Stopwatch _stopwatch;
    private int _stopTime;

    private void Awake()
    {
        _leftHandRootTransforms = leftHand.GetComponentsInChildren<Transform>();
        _rightHandRootTransforms = rightHand.GetComponentsInChildren<Transform>();
    }

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
            if (loop)
            {
                _playing = false;
                StartReplayTransforms();
                return;
            }
            //We're doing replaying, so stop the replay here
            gestureRecorderEvents.DoneReplayDemonstrationGesture();
            _playing = false;
            return;
        }
        _currentPoseFrameData = gestureRecorderEvents.CurrentGesture.GetPoseAtTime(Gesture.PoseDataType.Demonstration, _stopwatch.ElapsedMilliseconds);
        for (var i = 0; i < _currentRootTransforms.Length; i++)
        {
            _currentRootTransforms[i].SetPositionAndRotation(
                _currentPoseFrameData.positions[i],
                _currentPoseFrameData.rotations[i]);
        }
        _currentRootTransforms[0].position = _currentRootTransforms[0].position + _currentRootTransforms[0].position - GetCenterPosition();
    }

private Vector3 GetCenterPosition()
{
    var center = _currentRootTransforms.Aggregate(Vector3.zero, (current, t) => current + t.position);
    return center / (_currentRootTransforms.Length);
}
    private void StartReplayTransforms()
    {
        _playing = true;
        _currentIndex = 0;
        _stopwatch = Stopwatch.StartNew();
        _stopTime = gestureRecorderEvents.CurrentGesture.FrameCount(Gesture.PoseDataType.Demonstration);
    }
    private void OnEnable()
    {
        gestureRecorderEvents.OnStartReplayDemonstrationGesture += StartReplayTransforms;
        gestureRecorderEvents.OnDoneRecordingDemonstrationGesture += OnDemonstrationCreated;
        gestureRecorderEvents.OnGestureSelected += SetupDemonstrationHand;
        
    }

    private void OnDemonstrationCreated()
    {
        _currentRootTransforms = gestureRecorderEvents.CurrentGesture.Handedness == Handedness.Right ? _rightHandRootTransforms : _leftHandRootTransforms;
        _currentPoseFrameData = gestureRecorderEvents.CurrentGesture.GetPoseFrame(Gesture.PoseDataType.Demonstration, 0);
        for (var i = 0; i < _currentRootTransforms.Length; i++)
        {
            _currentRootTransforms[i].SetLocalPositionAndRotation(
                _currentPoseFrameData.positions[i],
                _currentPoseFrameData.rotations[i]);
        }
    }

    private void SetupDemonstrationHand(object obj)
    {
        if (gestureRecorderEvents.CurrentGesture.FrameCount(Gesture.PoseDataType.Demonstration) == 0)
        {
            gestureRecorderEvents.DoneReplayDemonstrationGesture();
            return;
        }
        _currentRootTransforms = gestureRecorderEvents.CurrentGesture.Handedness == Handedness.Right ? _rightHandRootTransforms : _leftHandRootTransforms;
        OnDemonstrationCreated();
        StartReplayTransforms();
    }

    private void OnDisable()
    {
        gestureRecorderEvents.OnStartReplayDemonstrationGesture -= StartReplayTransforms;
        gestureRecorderEvents.OnGestureSelected -= SetupDemonstrationHand;
        gestureRecorderEvents.OnDoneRecordingDemonstrationGesture -= OnDemonstrationCreated;

    }
}
