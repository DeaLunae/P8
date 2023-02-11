using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveToggle : MonoBehaviour
{
    private bool toggle = false;
    private void Start()
    {
        gameObject.SetActive(toggle);
    }
    public void SetActive(bool incomingToggle)
    {
        toggle = incomingToggle;
        gameObject.SetActive(toggle);
    }
}
