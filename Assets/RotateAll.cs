using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAll : MonoBehaviour
{
    private RotateAxis[] _rotates;

    private bool rotated;

    // Start is called before the first frame update
    void Start()
    {
        _rotates = FindObjectsOfType<RotateAxis>();
    }
    
    public void RotateAllObjects()
    {
        foreach (var rotate in _rotates)
        {
            rotate.Rotate();
        }
        rotated = !rotated;
    }

    public void ResetRotates()
    {
        if (!rotated) return;
        RotateAllObjects();
    }
}
