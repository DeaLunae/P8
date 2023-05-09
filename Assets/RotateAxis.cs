using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotateAxis : MonoBehaviour
{
    public bool3 axes = new(false, false, false);
    public void Rotate()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        if (axes.x)
        {
            rotation.x += 180;
        }
        if (axes.y)
        {
            rotation.y += 180;
        }
        if (axes.z)
        {
            rotation.z += 180;
        }
        transform.localRotation = Quaternion.Euler(rotation);
    }
}
