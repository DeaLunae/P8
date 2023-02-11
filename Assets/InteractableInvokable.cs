using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableInvokable : MonoBehaviour
{
    private RuntimeKnotGenerator _generator;
    private void Start()
    {
        _generator = GetComponentInParent<RuntimeKnotGenerator>();
    }

    public void OnKnotAdded(Vector3 position)
    {
        _generator.AddPointToEnd(position);
    }
    public void OnKnotDestroyed(GameObject gameObject)
    {
        _generator.OnKnotDestroyed(gameObject);
    }
    public void OnKnotMoved()
    {
        _generator.OnKnotMoved();
    }
}
