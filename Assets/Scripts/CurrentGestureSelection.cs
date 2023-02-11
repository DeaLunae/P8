using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CurrentGestureSelection : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvent;
    [SerializeField] private Transform currentSelectionQuad;
    [SerializeField] private float xScaleMax;
    [SerializeField] private float xScaleMin;
    [SerializeField] private float xPosMax;
    [SerializeField] private float xPosMin;

    private bool _Gestureinitialized;
    private int sampleFrameForThisGesture = 100;
    private int elapsedSampleFrames = 0;
    void OnEnable()
    {
        if (gestureRecorderEvent == null) return;
        gestureRecorderEvent.OnGestureSelected += UpdateCurrentGestureIndicator;
        gestureRecorderEvent.OnStartRecordingUserGesture += OnStartRecordingUserGesture;
    }

    private void OnStartRecordingUserGesture()
    {
        _Gestureinitialized = true;
        var progressBarTransform = currentSelectionQuad.transform;
        var position = progressBarTransform.localPosition;
        var scale = progressBarTransform.localScale;
        
        position = new Vector3(xPosMin, position.y, position.z);
        scale = new Vector3(xScaleMin, scale.y, scale.z);

        progressBarTransform.localPosition = position;
        progressBarTransform.localScale = scale;
    }
    void OnDisable()
    {
        if (gestureRecorderEvent == null) return;
        gestureRecorderEvent.OnGestureSelected -= UpdateCurrentGestureIndicator;
        gestureRecorderEvent.OnStartRecordingUserGesture -= OnStartRecordingUserGesture;

    }
    private void UpdateCurrentGestureIndicator(object obj)
    {
        if (obj is not Gesture gesture) return;
        print("GestureProgressUpdated");
        sampleFrameForThisGesture = gesture.amountOfSampleFrames;
        elapsedSampleFrames = 0;
    }
    
    private void Update()
    {
        if (_Gestureinitialized)
        {
            var newXPos = math.lerp(xPosMin, xPosMax, elapsedSampleFrames / (float)sampleFrameForThisGesture);
            var newXScale = math.lerp(xScaleMin, xScaleMax, elapsedSampleFrames / (float)sampleFrameForThisGesture);
            currentSelectionQuad.localPosition = new Vector3(newXPos, currentSelectionQuad.localPosition.y, currentSelectionQuad.localPosition.z);
            currentSelectionQuad.localScale = new Vector3(newXScale, currentSelectionQuad.localScale.y, currentSelectionQuad.localScale.z);

            elapsedSampleFrames++;
            
            if (elapsedSampleFrames == sampleFrameForThisGesture)
            {
                _Gestureinitialized = false;
            }
        }
    }

}
