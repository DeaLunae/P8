using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class MirrorAxis : MonoBehaviour
{
    [SerializeField] private bool3 axisToMirror;
    private Vector3 StartScale;
    
    private void Awake()
    {
        StartScale = transform.localScale;
    }
    public void Mirror()
    {
        transform.localScale = new Vector3(
            axisToMirror.x ? transform.position.x * -1 : StartScale.x,
            axisToMirror.y ? transform.position.y * -1 : StartScale.y,
            axisToMirror.z ? transform.position.z * -1 : StartScale.z);
    }

    public bool IsMirrored()
    {
        return StartScale == transform.localScale;
    }
}
