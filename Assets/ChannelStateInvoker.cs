using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Channels;
using UnityEngine;

public class ChannelStateInvoker : MonoBehaviour
{
    [SerializeField] private TransformChannel channel;
    [SerializeField] private bool _onStart;
    [SerializeField] private bool _onDestroy;
    [SerializeField] private bool _onEnable;
    [SerializeField] private bool _onDisable;


    private void Start()
    {
        if (_onStart)
        {
            channel.Value = transform;
        }
    }

    private void OnEnable()
    {
        if (_onEnable)
        {
            channel.Value = transform;
        }
    }
    

    private void OnDisable()
    {
        if (_onDisable)
        {
            channel.Value = null;
        }
    }

    private void OnDestroy()
    {
        if (_onDestroy)
        {
            channel.Value = null;
        }
    }
}
