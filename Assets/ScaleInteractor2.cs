using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ScaleInteractor2 : GenericInteractor
{
    [SerializeField] private InteractableReference currentSelection;
    [FormerlySerializedAs("safetyZone")] [SerializeField] private float _safetyZone = 0.3f;
    [SerializeField] private float _scalePerUnityMetre = 1f;
    private GenericInteractable _currentSelectionHolder;
    private bool locallyActive;
    private Vector3 lastHandPosition;
    private Vector3 _initialScale;
    private float _initialDistance;
    private ScaleConfiguration _configuration;
    private Transform _transform;
    private float _currentDistance;
    private float _percentageFromInitialDist;

    void Update()
    {
        if (!_interactorActive || !locallyActive)
        {
            return;
        }

        if (currentSelection.Value == null)
        {
            return;
        }
        if (!currentSelection.Value.BeingTranslated)
        {
            return;
        }
        if (_leftActivationGesture == Gesture.GestureID.Scale && Vector3.Distance(lastHandPosition, _leftHandTransform.position) > _safetyZone)
        {
            return;
        }
        if (_rightActivationGesture == Gesture.GestureID.Scale && Vector3.Distance(lastHandPosition, _rightHandTransform.position) > _safetyZone)
        {
            return;
        }

        if (_leftActivationGesture == Gesture.GestureID.Scale)
        {
            lastHandPosition = _leftHandTransform.position;
        }
        if (_rightActivationGesture == Gesture.GestureID.Scale)
        {
            lastHandPosition = _rightHandTransform.position;
        }
        _currentDistance = Vector3.Distance(lastHandPosition, _transform.position);
        _percentageFromInitialDist = _currentDistance / _initialDistance;
        var newscale = _scalePerUnityMetre * _percentageFromInitialDist * _initialScale;
        if (float.IsNaN(newscale.x) || float.IsNaN(newscale.y) || float.IsNaN(newscale.z))
        {
            newscale = _transform.localScale;
        }
        _configuration.Process(ref newscale);
        _transform.localScale = newscale;
    }
    
    private bool CheckIfActiveSelection()
    {
        return currentSelection.Value != null;
    }
    
    private void OnSelectionChanged(GenericInteractable genericInteractable)
    {
        //the selection changed
        if (_currentSelectionHolder != null)
        {
            //we already had a selectable tho
            if (_currentSelectionHolder == currentSelection.Value)
            {
                //but its the same one, so do nothing, we good
                return;
            }
            _currentSelectionHolder.StopScaling();
        }
        _currentSelectionHolder = currentSelection.Value;
    }
    private protected override void OnInteractorDisable()
    {
        locallyActive = false;
    }

    private protected override void OnInteractorEnable()
    {
        locallyActive = CheckIfActiveSelection();
        if (!locallyActive)
        {
            return;
        }
        if (!currentSelection.Value.StartScaling(ref _configuration, ref _transform))
        {
            locallyActive = false;
            return;
        }
        _initialScale = _transform.localScale;
        if (_leftActivationGesture == Gesture.GestureID.Scale)
        {
            lastHandPosition = _leftHandTransform.position;
            _initialDistance = Math.Clamp(Vector3.Distance(lastHandPosition, _transform.position),0.1f,2f);
        }

        if (_rightActivationGesture != Gesture.GestureID.Scale) return;
        lastHandPosition = _rightHandTransform.position;
        _initialDistance = Math.Clamp(Vector3.Distance(lastHandPosition, _transform.position),0.1f,2f);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        currentSelection.RegisterCallback(OnSelectionChanged);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        currentSelection.UnregisterCallback(OnSelectionChanged);
    }
}
