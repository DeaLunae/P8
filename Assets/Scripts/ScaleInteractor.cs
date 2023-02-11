using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

public class ScaleInteractor : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _leftScaleAnchor;
    [SerializeField] private ObjectEventReference _rightScaleAnchor;
    [SerializeField] private GestureIdReference _leftCurrentGesture;
    [SerializeField] private GestureIdReference _rightCurrentGesture;
    [SerializeField] private InteractableReference _currentSelectionRef;
    [SerializeField] private LineRenderer directionRenderer;

    private GenericInteractable _currentSelection;
    private bool _leftGestureActive,_rightGestureActive;
    private bool _leftTrackingActive,_rightTrackingActive;
    private Transform _leftAnchorTransform, _rightAnchorTransform;
    private Transform interactableTransform;
    private ScaleConfiguration _config;
    private bool scalingActive;
    private Vector3 initialScale, newScale;
    private float initialScaleDist, distance, percentageDiffFromInitial;

    private void Update()
    {
        if (!_leftTrackingActive || !_rightTrackingActive || !scalingActive) { return; }
        
        directionRenderer.SetPosition(0,_leftAnchorTransform.position);
        directionRenderer.SetPosition(1,_rightAnchorTransform.position);
        
        distance = Vector3.Distance(_leftAnchorTransform.position,_rightAnchorTransform.position);
        percentageDiffFromInitial = distance / initialScaleDist;
        newScale = percentageDiffFromInitial * initialScale;
        
        _config.Process(ref newScale);
        interactableTransform.localScale = newScale;
    }
    
    #region EventRegistration
    private void OnEnable()
    {
        _leftScaleAnchor.RegisterListener(OnLeftAnchorCreate, OnLeftAnchorDestroy);
        _rightScaleAnchor.RegisterListener(OnRightAnchorCreate, OnRightAnchorDestroy);
        _leftCurrentGesture.RegisterCallback(OnLeftGestureChanged);
        _rightCurrentGesture.RegisterCallback(OnRightGestureChanged);
        _currentSelectionRef.RegisterCallback(OnSelectionChanged);
    }

    private void OnSelectionChanged(GenericInteractable genericInteractable)
    {
        if (_currentSelection == _currentSelectionRef.Value)
        {
            return;
        }
        if (_currentSelection != null)
        {
            _currentSelection.StopScaling();
        }
        _currentSelection = _currentSelectionRef.Value;
        ConditionalActivation();
    }

    private void OnDisable()
    {
        _leftScaleAnchor.UnregisterListener(OnLeftAnchorCreate, OnLeftAnchorDestroy);
        _rightScaleAnchor.UnregisterListener(OnRightAnchorCreate, OnRightAnchorDestroy);
        _leftCurrentGesture.UnregisterCallback(OnLeftGestureChanged);
        _rightCurrentGesture.UnregisterCallback(OnRightGestureChanged);
        _currentSelectionRef.UnregisterCallback(OnSelectionChanged);
    }
    #endregion
    #region EventCallbacks

    private void OnLeftGestureChanged(Gesture.GestureID gestureID)
    {
        _leftGestureActive = _leftCurrentGesture == Gesture.GestureID.Scale;
        ConditionalActivation();
    }
    private void OnRightGestureChanged(Gesture.GestureID gestureID)
    {
        _rightGestureActive = _rightCurrentGesture == Gesture.GestureID.Scale;
        ConditionalActivation();
    }

    private void ConditionalActivation()
    {
        if (!_leftGestureActive || !_rightGestureActive || !_rightTrackingActive || !_leftTrackingActive)
        {
            scalingActive = false;
            if (_currentSelection != null)
            {
                _currentSelection.StopScaling();
            }
            return;
        }
        if (_currentSelection == null)
        {
            return;
        }
        scalingActive = _currentSelection.VerifyOwnership(ref interactableTransform) ||
                        _currentSelection.StartScaling(ref _config, ref interactableTransform);
        if (!scalingActive) return;
        initialScale = interactableTransform.localScale;
        initialScaleDist = Vector3.Distance(_leftAnchorTransform.position, _rightAnchorTransform.position);
    }

    private void OnLeftAnchorCreate(Object obj)
    {
        _leftAnchorTransform = ((GameObject)obj).transform;
        _leftTrackingActive = true;
    }
    private void OnLeftAnchorDestroy(Object obj)
    {
        _leftAnchorTransform = null;
        _leftTrackingActive = false;
    }
    private void OnRightAnchorCreate(Object obj)
    {
        _rightAnchorTransform = ((GameObject)obj).transform;
        _rightTrackingActive = true;
    }
    private void OnRightAnchorDestroy(Object obj)
    {
        _rightAnchorTransform = null;
        _rightTrackingActive = false;
    }
    #endregion
}