using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;
using Object = UnityEngine.Object;

public class UpdateTransform : MonoBehaviour
{
    [SerializeField] private ObjectEventReference camera;

    [SerializeField,ReadOnly] private bool isTracked;
    private Transform trackedTransform;
    
    
    // Update is called once per frame
    void Update()
    {
        if (isTracked)
        {
            transform.position = -trackedTransform.localPosition;
        }
    }
    private void OnEnable()
    {
        camera.RegisterListenerExtended(OnCameraCreated,OnCameraDestroyed,OnCameraEnabled,OnCameraDisabled);
    }

    private void OnDisable()
    {
        camera.UnregisterListenerExtended(OnCameraCreated,OnCameraDestroyed,OnCameraEnabled,OnCameraDisabled);
    }

    private void OnCameraCreated(Object obj)
    {
        isTracked = true;
        trackedTransform = ((GameObject)obj).transform;
    }

    private void OnCameraDestroyed(Object obj)
    {
        isTracked = false;
        trackedTransform = null;
    }

    private void OnCameraEnabled(Object obj)
    {
        isTracked = true;
        trackedTransform = ((GameObject)obj).transform;
    }

    private void OnCameraDisabled(Object obj)
    {
        isTracked = false;
        trackedTransform = null;
    }
}
