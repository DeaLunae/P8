using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDestroyer : MonoBehaviour
{
    [SerializeField] private InteractableReference _currentSelection;

    public void DestroySelection()
    {
        if (_currentSelection.Value == null) { return; }
        if (!_currentSelection.Value.Destroyable()) { return; }
        _currentSelection.Value.StopHover();
        _currentSelection.Value.StopSelect();
        Destroy(_currentSelection.Value.gameObject);
        _currentSelection.Value = null;
    }
}
