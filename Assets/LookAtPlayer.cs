using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Vector3 posholder;
    void Update()
    {
        if (CameraCache.Main != null)
        {
            posholder = CameraCache.Main.transform.position;
            posholder.y = transform.position.y;
            transform.LookAt(posholder, Vector3.forward);
        }
    }
}
