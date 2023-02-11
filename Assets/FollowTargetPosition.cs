using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class FollowTargetPosition : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private ObjectReference _followTarget;
    [SerializeField] private Transform _thingThatShouldFollow;
    [SerializeField] private Vector3 offsetPos;

    private bool followActive;
    private Transform followTargetTransform;

    private void OnEnable()
    {
        _followTarget.RegisterCallback(OnTargetChanged);
    }
    private void OnDisable()
    {
        _followTarget.UnregisterCallback(OnTargetChanged);
    }
    private void OnTargetChanged(Object o)
    {
        followActive = _followTarget.Value != null;
        followTargetTransform = followActive ? ((GameObject)_followTarget.Value).transform : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (followActive)
        {
            _thingThatShouldFollow.position = Vector3.MoveTowards(_thingThatShouldFollow.transform.position, followTargetTransform.position,
                Time.deltaTime * speed);
            _thingThatShouldFollow.rotation = Quaternion.LookRotation(_thingThatShouldFollow.position - CameraCache.Main.transform.position);
        }
    }
}
