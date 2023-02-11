using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    private bool toggle = false;
    private void Start()
    {
        Invoke(nameof(DelayedDisable),0f);
    }

    private void DelayedDisable()
    {
        menu.SetActive(false);
    }
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start,OVRInput.Controller.LTouch))
        {
            toggle = !toggle;
            menu.SetActive(toggle);
        }
    }
}
