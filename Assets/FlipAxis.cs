using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlipAxis : MonoBehaviour
{
    public bool3 axes = new(false, false, false);
    public void Flip()
    {
        Vector3 scale = transform.localScale;
        if (axes.x)
        {
            scale.x *= -1;
        }
        if (axes.y)
        {
            scale.y *= -1;
        }
        if (axes.z)
        {
            scale.z *= -1;
        }
        transform.localScale = scale;
    }
}
