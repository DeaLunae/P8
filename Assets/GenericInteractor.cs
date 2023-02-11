using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
using HandJointUtils = Microsoft.MixedReality.Toolkit.Input.HandJointUtils;
using Object = UnityEngine.Object;

public abstract class GenericInteractor : MonoBehaviour	
{
    [Header("General")]
    [SerializeField] protected Handedness _handedness;
    [SerializeField] protected EditingStateReference _editingStateReference;
    [SerializeField] protected AnimationStateReference _animationStateReference;
    [Header("Left interactor")]
    [SerializeField] protected ObjectEventReference _leftInteractorReference;
    [SerializeField] protected GestureIdReference _leftInteractorGesture;
    [SerializeField] protected bool _leftExceptMode;
    [SerializeField] protected Gesture.GestureID _leftActivationGesture;
    [FormerlySerializedAs("_rightHandReference")]
    [Header("Right Interactor")]
    [SerializeField] protected ObjectEventReference _rightInteractorReference;
    [FormerlySerializedAs("_rightHandGesture")] [SerializeField] protected GestureIdReference _rightInteractorGesture;
    [SerializeField] protected bool _rightExceptMode;
    [SerializeField] protected Gesture.GestureID _rightActivationGesture;
    [SerializeField] protected IntReference _condition;
    
    protected Transform _leftHandTransform;
    protected Transform _rightHandTransform;

    protected bool _leftGestureActive;
    protected bool _rightGestureActive;
    protected bool _interactorActive;
    
    protected IMixedRealityController leftInteractor, rightInteractor;
    public void OnConditionsChanged()
    {
        bool interactorShouldBeActive;
        switch (_handedness)
        {
            case Handedness.Left:
            {
                if (_condition.Value == 0)
                {
 
                    leftInteractor = HandJointUtils.FindHand(Handedness.Left);
                    if (leftInteractor == null || leftInteractor.TrackingState == TrackingState.NotTracked)
                    {
                        interactorShouldBeActive = false;
                        break;
                    }
                }
                interactorShouldBeActive = _leftGestureActive && (!_rightExceptMode || _rightGestureActive) &&
                                           _leftHandTransform != null &&
                                           _animationStateReference.Value == AnimationState.Stopped;
                break;
        }
            case Handedness.Right:
            {
                if (_condition.Value == 0)
                {
                    rightInteractor = HandJointUtils.FindHand(Handedness.Right);
                    if (rightInteractor == null || rightInteractor.TrackingState == TrackingState.NotTracked)
                    {
                        interactorShouldBeActive = false;
                        break;
                    }
                }
                interactorShouldBeActive = _rightGestureActive && 
                                           (!_leftExceptMode || _leftGestureActive) &&
                                           _rightHandTransform != null &&
                                           _animationStateReference.Value == AnimationState.Stopped;
                break;
            }
            case Handedness.Both:
            {
                if (_condition.Value == 0)
                {
                    leftInteractor = HandJointUtils.FindHand(Handedness.Left);
                    if (leftInteractor == null || leftInteractor.TrackingState == TrackingState.NotTracked)
                    {
                        interactorShouldBeActive = false;
                        break;
                    }                    
                    rightInteractor = HandJointUtils.FindHand(Handedness.Right);
                    if (rightInteractor == null || rightInteractor.TrackingState == TrackingState.NotTracked)
                    {
                        interactorShouldBeActive = false;
                        break;
                    }
                }
                interactorShouldBeActive = _leftGestureActive && 
                                           _rightGestureActive && 
                                           _leftHandTransform != null &&
                                           _rightHandTransform != null &&
                                           _animationStateReference.Value == AnimationState.Stopped;
                break;
            }
            default:
                interactorShouldBeActive = false;
                break;
        }

        if (!interactorShouldBeActive)
        {
            if (!_interactorActive) return;
            OnInteractorDisable();
            _interactorActive = false;

        }
        else
        {
            if (_interactorActive) return;
            OnInteractorEnable();
            _interactorActive = true;
        }
    }
    
    private protected abstract void OnInteractorDisable();
    private protected abstract void OnInteractorEnable();
    
    protected virtual void OnEnable()
    {
        switch (_handedness)
        {
            case Handedness.Left:
                _leftInteractorGesture.RegisterCallback(OnLeftGestureChanged);
                _leftInteractorReference.RegisterListenerExtended(OnLeftHandCreate,OnLeftHandDestroyed,OnLeftHandEnabled,OnLeftHandDisabled);
                break;
            case Handedness.Right:
                _rightInteractorGesture.RegisterCallback(OnRightGestureChanged);
                _rightInteractorReference.RegisterListenerExtended(OnRightHandCreate,OnRightHandDestroyed,OnRightHandEnabled,OnRightHandDisabled);
                break;
            case Handedness.Both:
                _leftInteractorGesture.RegisterCallback(OnLeftGestureChanged);
                _rightInteractorGesture.RegisterCallback(OnRightGestureChanged);
                _leftInteractorReference.RegisterListenerExtended(OnLeftHandCreate,OnLeftHandDestroyed,OnLeftHandEnabled,OnLeftHandDisabled);
                _rightInteractorReference.RegisterListenerExtended(OnRightHandCreate,OnRightHandDestroyed,OnRightHandEnabled,OnRightHandDisabled);
                break;
        }
        _editingStateReference.RegisterCallback(OnEditStateChanged);
        _animationStateReference.RegisterCallback(OnAnimationStateChanged);
    }
    protected virtual void OnDisable()
    {
        switch (_handedness)
        {
            case Handedness.Left:
                _leftInteractorGesture.UnregisterCallback(OnLeftGestureChanged);
                _leftInteractorReference.UnregisterListenerExtended(OnLeftHandCreate,OnLeftHandDestroyed,OnLeftHandEnabled,OnLeftHandDisabled);
                break;
            case Handedness.Right:
                _rightInteractorGesture.UnregisterCallback(OnRightGestureChanged);
                _rightInteractorReference.UnregisterListenerExtended(OnRightHandCreate,OnRightHandDestroyed,OnRightHandEnabled,OnRightHandDisabled);
                break;
            case Handedness.Both:
                _leftInteractorGesture.UnregisterCallback(OnLeftGestureChanged);
                _rightInteractorGesture.UnregisterCallback(OnRightGestureChanged);
                _leftInteractorReference.UnregisterListenerExtended(OnLeftHandCreate,OnLeftHandDestroyed,OnLeftHandEnabled,OnLeftHandDisabled);
                _rightInteractorReference.UnregisterListenerExtended(OnRightHandCreate,OnRightHandDestroyed,OnRightHandEnabled,OnRightHandDisabled);
                break;
        }
        _editingStateReference.UnregisterCallback(OnEditStateChanged);
        _animationStateReference.UnregisterCallback(OnAnimationStateChanged);
    }

    private void OnAnimationStateChanged(AnimationState animationState)
    {
        OnConditionsChanged();
    }
    
    private void OnEditStateChanged(EditingState editingState)
    {
        OnConditionsChanged();
    }

    private void OnRightHandDisabled(Object obj)
    {
        _rightHandTransform = null;
        OnConditionsChanged();
    }

    private void OnRightHandEnabled(Object obj)
    {
        _rightHandTransform = ((GameObject)obj).transform;
        OnConditionsChanged();
    }

    private void OnRightHandDestroyed(Object obj)
    {
        _rightHandTransform = null;
        OnConditionsChanged();
    }

    private void OnRightHandCreate(Object obj)
    {
        _rightHandTransform = ((GameObject)obj).transform;
        OnConditionsChanged();
    }

    private void OnLeftHandDisabled(Object obj)
    {
        _leftHandTransform = null;

        OnConditionsChanged();
    }

    private void OnLeftHandEnabled(Object obj)
    {
        _leftHandTransform = ((GameObject)obj).transform;
        OnConditionsChanged();
    }

    private void OnLeftHandDestroyed(Object obj)
    {
        _leftHandTransform = null;
        OnConditionsChanged();
    }

    private void OnLeftHandCreate(Object obj)
    {
        _leftHandTransform = ((GameObject)obj).transform;
        OnConditionsChanged();
    }
    protected virtual void OnLeftGestureChanged(Gesture.GestureID gestureID)
    {
        if (_leftExceptMode)
        {
            _leftGestureActive = _leftInteractorGesture.Value != _leftActivationGesture;
        }
        else
        {
            _leftGestureActive = _leftInteractorGesture.Value == _leftActivationGesture;
        }
        OnConditionsChanged();
    }
    protected virtual void OnRightGestureChanged(Gesture.GestureID gestureID)
    {
        if (_rightExceptMode)
        {
            _rightGestureActive = _rightInteractorGesture.Value != _rightActivationGesture;
        }
        else
        {
            _rightGestureActive = _rightInteractorGesture.Value == _rightActivationGesture;
        }
        OnConditionsChanged();
    }

  
}
