using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Devkit.Modularis.Variables;
using UnityEngine;

public class SunTask : MonoBehaviour
{
    [SerializeField] private StringReference task;
    [SerializeField] private Vector3 minScaleToVerify;

    public void OnScaleEnded(Vector3 scale)
    {
        if (scale.x >= minScaleToVerify.x)
        {
            task.Value = task.Value;
        }
    }
}
