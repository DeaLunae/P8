using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using Object = UnityEngine.Object;

public class SetSolverHandlerTransform : MonoBehaviour
{
    [SerializeField] private ObjectReference controllerTransform;

    private SolverHandler _solverHandler;
    // Start is called before the first frame update
    private void OnEnable()
    {
        controllerTransform.RegisterCallback(ControllerStatusChanged);
    }
    private void OnDisable()
    {
        controllerTransform.UnregisterCallback(ControllerStatusChanged);
    }
    private void ControllerStatusChanged(Object o)
    {
        if (controllerTransform.Value == null)
        {
            _solverHandler.TransformOverride = null;
        }
        else
        {
            _solverHandler.TransformOverride = ((GameObject)controllerTransform).transform;
        }
    }

    void Start()
    {
        _solverHandler = GetComponent<SolverHandler>();

    }
}
