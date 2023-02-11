using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;

public class RegisterObject : MonoBehaviour
{
    [SerializeField] private ObjectReference handObjectRef;
    [SerializeField] private GameObject gameObjectToRegister;
    
    private void OnEnable()
    {
        handObjectRef.Value = gameObjectToRegister;
    }

    private void OnDestroy()
    {
        handObjectRef.Value = null;
    }
}
