using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devkit.Modularis.Channels;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

public class UserPoseRecorder : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    [SerializeField] private TransformChannel leftHandTransform,rightHandTransform;
    private TransformChannel _correctTransform;
    private Transform[] _transforms;
    private bool _isRecording;
    private float _recordingStartTime;
    private long _stoptime;


    private void OnHandRootTransformChanged()
    {
        if (!_correctTransform.isSet)
        {
            _transforms = null;
            return;
        }
        _transforms = _correctTransform.Value.GetComponentsInChildren<Transform>();
    }


    private void BeginRecording()
    {
        print("Begin Recording");
        if (!_correctTransform.isSet)
        {
            return;
        }
        _gestureRecorderEvents.CurrentGesture.ResetPoseData(Gesture.PoseDataType.User);
        _isRecording = true;
        _recordingStartTime = Time.time;
    }

    private void OnDestroy()
    {
        _gestureRecorderEvents.CurrentGesture.ResetPoseData(Gesture.PoseDataType.User);
    }

    private void StopRecording()
    {

        _isRecording = false;
    }
    private void Update()
    {
        if (_isRecording && _correctTransform.isSet)
        {
            _gestureRecorderEvents.CurrentGesture.AddPoseFrame(Gesture.PoseDataType.User, new Gesture.PoseFrameData
            {
                positions = _transforms.Select(e => e.localPosition).ToList(),
                rotations = _transforms.Select(e => e.localRotation).ToList(),
                time = Time.time - _recordingStartTime
            });
        }
    }

    private void OnEnable()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnStartRecordingUserGesture += BeginRecording;
            _gestureRecorderEvents.OnDoneRecordingUserGesture += StopRecording;
            _gestureRecorderEvents.OnGestureSelected += UpdateTransforms;
            _correctTransform.RegisterCallback(OnHandRootTransformChanged);
        }
    }

    private void UpdateTransforms(object obj)
    {
        if (_correctTransform != null)
        {
            _correctTransform.UnregisterCallback(OnHandRootTransformChanged);
        }
        _correctTransform = _gestureRecorderEvents.CurrentGesture.Handedness == Handedness.Right ? rightHandTransform : leftHandTransform;
        _correctTransform.RegisterCallback(OnHandRootTransformChanged);
        OnHandRootTransformChanged();
    }

    private void OnDisable()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnStartRecordingUserGesture -= BeginRecording;
            _gestureRecorderEvents.OnDoneRecordingUserGesture -= StopRecording;
            _gestureRecorderEvents.OnGestureSelected -= UpdateTransforms;
            _correctTransform.UnregisterCallback(OnHandRootTransformChanged);
        }

    }

    private Vector3 GetCenterPosition(Transform t)
    {
        var children = t.GetComponentsInChildren<Transform>();
        var center = children.Aggregate(Vector3.zero, (current, t) => current + t.position);
        return center / (children.Length);
    }
}
