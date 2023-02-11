using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectEvent : MonoBehaviour
{
    [SerializeField] private ObjectEventReference objectEventReference;
    [SerializeField] private bool _onCreate;
    [SerializeField] private bool _onDestroy;
    [SerializeField] private bool _onEnable;
    [SerializeField] private bool _onDisable;

    
    void Awake()
    {
        if (_onCreate)
        {
            objectEventReference.ObjectCreated(gameObject);
        }
    }

    private void OnEnable()
    {
        if (_onEnable)
        {
            objectEventReference.ObjectEnabled(gameObject);
        }
    }

    private void OnDisable()
    {
        if (_onDisable)
        {
            objectEventReference.ObjectDisabled(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_onDestroy)
        {
            objectEventReference.ObjectDestroyed(gameObject);
        }
    }
}
