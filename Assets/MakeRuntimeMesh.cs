using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRuntimeMesh : MonoBehaviour
{
    private void Reset()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }
    }

    private void Awake()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }
        
    }
}
