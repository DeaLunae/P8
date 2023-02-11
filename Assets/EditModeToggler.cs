using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeToggler : MonoBehaviour
{
    [SerializeField] private EditingStateVariable _editingState;

    public void SetObjectEditState()
    {
        _editingState.Value = EditingState.Object;
    } 
    public void SetSplineEditState()
    {
        _editingState.Value = EditingState.Spline;
    }
}
