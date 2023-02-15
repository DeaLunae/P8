using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDisableOnStart : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(DelayedMethod),0.1f);
    }
    public void DelayedMethod()
    {
        gameObject.SetActive(false);
    }
}
