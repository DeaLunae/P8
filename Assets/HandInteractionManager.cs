using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Devkit.Modularis.Variables;
using UnityEngine;
using Object = UnityEngine.Object;

public class HandInteractionManager : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _rightHandReference,_leftHandReference;
    [SerializeField] private BoolVariable _rightHandActive, _leftHandActive;
    private GameObject _rightHandGo, _leftHandGo;
    [SerializeField] private Gesture.GestureID _gestureID;
    
    private void Start()
    {
        _rightHandReference.RegisterListenerExtended(OnRightHandSpawned,OnRightHandDestroyed,OnRightHandEnabled,OnRightHandDisabled);
        _leftHandReference.RegisterListenerExtended(OnLeftHandSpawned,OnLeftHandDestroyed,OnLeftHandEnabled,OnLeftHandDisabled);
    }
    
    private void OnRightHandSpawned(Object o)
    {
        _rightHandGo = (GameObject)o;
        _rightHandActive.Value = true;
    }
    private void OnRightHandDestroyed(Object o)
    {
        _rightHandGo = null;
        _rightHandActive.Value = false;
    }
    private void OnLeftHandSpawned(Object o)
    {
        _leftHandGo = (GameObject)o;
        _leftHandActive.Value = true;
    }
    private void OnLeftHandDestroyed(Object o)
    {
        _leftHandGo = null;
        _leftHandActive.Value = false;
    }
    private void OnRightHandEnabled(Object o)
    {
        _rightHandActive.Value = true;
    }
    private void OnRightHandDisabled(Object o)
    {
        _rightHandActive.Value = false;
    }
    private void OnLeftHandEnabled(Object o)
    {
        _leftHandActive.Value = true;
    }
    private void OnLeftHandDisabled(Object o)
    {
        _leftHandActive.Value = false;
    }
    
}
