using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Devkit.Modularis.Channels;
using UnityEditor;

public class DemonstrationPoseRecorder : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    [SerializeField] private TransformChannel _HandRootTransform;
    private IEnumerable<Transform> _transforms;
    private bool _isRecording;
    private float _recordingStartTime;


    private void OnHandRootTransformChanged()
    {
        if (!_HandRootTransform.isSet)
        {
            _transforms = null;
            return;
        }
        _transforms = _HandRootTransform.Value.GetComponentsInChildren<Transform>();
    }


    private void BeginRecording()
    {
        if (!_HandRootTransform.isSet)
        {
            _gestureRecorderEvents.DoneRecordingDemonstrationGesture();
            return;
        }
        _gestureRecorderEvents.CurrentGesture.ResetPoseData();
        if (_gestureRecorderEvents.CurrentGesture.Type == Gesture.GestureType.Static)
        {
            _gestureRecorderEvents.CurrentGesture.AddPoseFrame(Gesture.PoseDataType.Demonstration,new Gesture.PoseFrameData
            {
                positions = _transforms.Select(e => e.position).ToList(),
                rotations = _transforms.Select(e => e.rotation).ToList(),
                time = 0
            });
            _gestureRecorderEvents.DoneRecordingDemonstrationGesture();
            return;
        }
        _isRecording = true;
        _recordingStartTime = Time.time;
    }

    private void StopRecording()
    {
        _isRecording = false;
    }
    private void FixedUpdate()
    {
        if (_HandRootTransform.isSet && _isRecording)
        {
            _gestureRecorderEvents.CurrentGesture.AddPoseFrame(Gesture.PoseDataType.Demonstration, new Gesture.PoseFrameData
            {
                positions = _transforms.Select(e => e.position).ToList(),
                rotations = _transforms.Select(e => e.rotation).ToList(),
                time = Time.time - _recordingStartTime
            });
        }
    }

    private void OnEnable()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnStartRecordingDemonstrationGesture += BeginRecording;
            _gestureRecorderEvents.OnDoneRecordingDemonstrationGesture += StopRecording;
        }
        if (_HandRootTransform != null)
        {
            _HandRootTransform.RegisterCallback(OnHandRootTransformChanged);
        }
    }
    private void OnDisable()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnStartRecordingDemonstrationGesture -= BeginRecording;
            _gestureRecorderEvents.OnDoneRecordingDemonstrationGesture -= StopRecording;
        }
        if (_HandRootTransform != null)
        {
            _HandRootTransform.UnregisterCallback(OnHandRootTransformChanged);
        }
    }

    private Vector3 GetCenterPosition(Transform t)
    {
        var children = t.GetComponentsInChildren<Transform>();
        var center = children.Aggregate(Vector3.zero, (current, t) => current + t.position);
        return center / (children.Length);
    }
}
