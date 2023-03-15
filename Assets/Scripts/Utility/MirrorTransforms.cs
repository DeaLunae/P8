using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Mathematics;

public class MirrorTransforms : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ObjectEventReference objectEventReference;
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    [SerializeField] private Transform handRoot;
    [SerializeField] private Handedness hand;
    
    private Transform _anchorTransform;
    private bool _mirroringActive,_replayingActive;
    private Transform[] _originTransforms;
    private Transform[] _targetTransforms;
    private bool doesRecordingExist;
    private Vector3[] _currentRecordingPosition;
    private Quaternion[] _currentRecordingRotation;
    private Stopwatch _stopwatch;
    private float _stopTime;
    private Gesture.PoseFrameData _currentPoseFrameData;
    private int _currentIndex;
    private Vector3 _timeSeriesCenter;

    void Start()
    {
        _anchorTransform = transform.parent;
        _targetTransforms = transform.GetComponentsInChildren<Transform>();
        _targetTransforms[0].rotation = Quaternion.identity;
    }
    void Update()
    {
        if (_replayingActive)
        {
            PlayNextTransformPose();
        } else if (_mirroringActive)
        {
            MirrorCurrentTransform();
        }
    }

    private void MirrorCurrentTransform()
    {
        _targetTransforms[0].localRotation = _originTransforms[0].localRotation;
        transform.position = _anchorTransform.position;
        for (var i = 1; i < _originTransforms.Length; i++)
        {
            _targetTransforms[i].localPosition = _originTransforms[i].localPosition;
            _targetTransforms[i].localRotation = _originTransforms[i].localRotation;
        }

        transform.position = _anchorTransform.position + _anchorTransform.position - GetCenterPosition();
    }
    
    private void PlayNextTransformPose()
    {
        if (_stopwatch.ElapsedMilliseconds >= _stopTime)
        {
            //We're doing replaying, so stop the replay here
            _gestureRecorderEvents.DoneReplayRecordedUserGesture();
            _replayingActive = false;
            return;
        }
        _currentPoseFrameData = _gestureRecorderEvents.CurrentGesture.GetPoseAtTime(Gesture.PoseDataType.User, _stopwatch.ElapsedMilliseconds/1000f);
        for (var i = 0; i < _targetTransforms.Length; i++)
        {
            _targetTransforms[i].SetLocalPositionAndRotation(
                _currentPoseFrameData.positions[i],
                _currentPoseFrameData.rotations[i]);
        }
        _targetTransforms[0].position = _anchorTransform.position + (_currentPoseFrameData.positions[0] - _timeSeriesCenter);
        //transform.position = _anchorTransform.position + _anchorTransform.position - GetCenterPosition();
    }
    
    private Vector3 GetCenterPosition()
    {
        var center = _targetTransforms.Aggregate(Vector3.zero, (current, t) => current + t.position);
        return center / (_targetTransforms.Length);
    }
    
    private void OnEnable()
    {
        CheckIfHandsAlreadySpawned();
        _gestureRecorderEvents.OnGestureSelected += SubscribeIfCorrectHand;

        if (objectEventReference == null) return;
        objectEventReference.RegisterListener(onCreateMethod: OnHandSpawned, onDestroyedMethod: OnHandDestroyed);
    }

    private void CheckIfHandsAlreadySpawned()
    {
        if (_mirroringActive) return;
        var objevents = FindObjectsOfType<ObjectEvent>();
        if (objevents.Length <= 0) return;
        var appropriateHand = objevents.FirstOrDefault(e => e.gameObject.name.StartsWith(gameObject.name[..1]));
        if (appropriateHand == null) return;
        OnHandSpawned(appropriateHand.gameObject);
    }
    
    private void StartReplayTransforms()
    {
        if (_gestureRecorderEvents.CurrentGesture.FrameCount(Gesture.PoseDataType.User) == 0)
        {
            Invoke(nameof(DelayedDone),1f);
            return;
        }
        _replayingActive = true;
        _currentIndex = 0;
        _stopwatch = Stopwatch.StartNew();
        transform.position = _anchorTransform.position + _anchorTransform.position - GetCenterPosition();
        _stopTime = _gestureRecorderEvents.CurrentGesture.GetTotalTime(Gesture.PoseDataType.User) * 1000;
        _timeSeriesCenter = CalculateCenterOfTimeSeries();
    }

    private Vector3 CalculateCenterOfTimeSeries()
    {
        List<Gesture.PoseFrameData> timeseries = _gestureRecorderEvents.CurrentGesture.UserData;
        List<Vector3> wristPositions = timeseries.Select(e => e.positions[0]).ToList();
        return wristPositions.Average();
    }
   
    private void DelayedDone()
    {
        _gestureRecorderEvents.DoneReplayRecordedUserGesture();

    }
    private void OnDisable()
    {
        _gestureRecorderEvents.OnStartReplayRecordedUserGesture -= StartReplayTransforms;
        _gestureRecorderEvents.OnGestureSelected -= SubscribeIfCorrectHand;
        if (objectEventReference == null) return;
        objectEventReference.UnregisterListener(OnHandSpawned, OnHandDestroyed);
    }
    
    private void SubscribeIfCorrectHand(object obj)
    {
        _gestureRecorderEvents.OnStartReplayRecordedUserGesture -= StartReplayTransforms;
        if (_gestureRecorderEvents.CurrentGesture.Handedness != hand) { return; }
        _gestureRecorderEvents.OnStartReplayRecordedUserGesture += StartReplayTransforms;
    }
    
    private void OnHandSpawned(object obj)
    {
        if (obj is not GameObject go) return;
        _originTransforms = go.transform.GetComponentsInChildren<Transform>();
        _mirroringActive = true;
    }
    
    private void OnHandDestroyed(object obj)
    {
        if (obj is not GameObject go) return;
        //var currentHand = programSo.program[_currProgramIndex].handedness == Handedness.Right ? "R_" : "L_";
        _mirroringActive = false;
    }
}
