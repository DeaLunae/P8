using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MirrorTransforms : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ObjectEventReference objectEventReference;
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    private Transform _anchorTransform;
    private bool _mirroringActive,_replayingActive;

    private Transform[] _originTransforms;
    private Transform[] _targetTransforms;

    private bool doesRecordingExist;
    private Vector3[] _currentRecordingPosition;
    private Quaternion[] _currentRecordingRotation;
    void Start()
    {
        _anchorTransform = transform.parent;
        if (objectEventReference == null) return;
        objectEventReference.RegisterListener(onCreateMethod: OnHandSpawned, onDestroyedMethod: OnHandDestroyed);
        _gestureRecorderEvents.OnStartReplayRecordedUserGesture += OnReplayStarted;
        _targetTransforms = transform.GetComponentsInChildren<Transform>();
        _targetTransforms[0].rotation = Quaternion.identity;

    }

    private void OnReplayStarted()
    {
        
    }
    private void OnEnable()
    {
        CheckIfHandsAlreadySpawned();
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

    private void OnDestroy()
    {
        if (objectEventReference == null) return;
        objectEventReference.UnregisterListener(onCreateMethod: OnHandSpawned, onDestroyedMethod: OnHandDestroyed);
    }

    // Update is called once per frame
    void Update()
    {
        if (_mirroringActive)
        {
            _targetTransforms[0].localRotation = _originTransforms[0].localRotation;
            transform.position = _anchorTransform.position;
            for (var i = 1; i < _originTransforms.Length; i++)
            {
                _targetTransforms[i].localPosition = _originTransforms[i].localPosition;
                _targetTransforms[i].localRotation = _originTransforms[i].localRotation;
            }

            transform.position = _anchorTransform.position + _anchorTransform.position - GetCenterPosition();
        }else if (_replayingActive)
        {
            
        }
    }

    private Vector3 GetCenterPosition()
    {
        var center = _targetTransforms.Aggregate(Vector3.zero, (current, t) => current + t.position);
        return center / (_targetTransforms.Length);
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
