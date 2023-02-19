using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepEnabledIfEditor : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }
}
