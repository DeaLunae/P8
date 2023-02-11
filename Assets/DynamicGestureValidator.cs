using System;
using System.Collections.Generic;
using System.Linq;
using Devkit.Modularis.Events;
using Devkit.Modularis.References;
using Unity.Mathematics;
using UnityEngine;

public class DynamicGestureValidator : MonoBehaviour
{
    [SerializeField, Range(0,1f)] 
    private float gestureInitializationReq;

    [SerializeField, Range(0, 1f)] 
    private float gestureMaintenanceReq;
    
    
    [SerializeField] 
    private int framesToConsider;

    [SerializeField]
    private AnimationCurve confidenceWeighing;
    
    private Dictionary<int, List<Detection>> _detectionFrame;
    [SerializeField] private bool gestureOverrideMode;
    [SerializeField] private Detection _currentGesture = new Detection(Gesture.GestureID.None,0f);
    [SerializeField] private Detection _lastGesture = new Detection(Gesture.GestureID.None,0f);

    [SerializeField,ReadOnly] private List<Detection> currentGestureScores = new ();

    [SerializeField] private GestureIdReference _gestureIdReference;
    [SerializeField] private GameEvent _GestureStarted;
    [SerializeField] private GameEvent _GestureStopped;
    
    [Serializable]
    public struct Detection
    {
        public Gesture.GestureID gestureID;
        [Range(0f,1f)] public float confidence;
        public Detection(Gesture.GestureID gestureID, float confidence)
        {
            this.gestureID = gestureID;
            this.confidence = confidence;
        }
    }
    private static readonly List<string> GestureIDList = Enum.GetNames(typeof(Gesture.GestureID)).ToList();

    public void OrderedStart()
    {
        _detectionFrame = new Dictionary<int, List<Detection>>();
        for (int i = 0; i < GestureIDList.Count; i++)
        {
            _detectionFrame[i] = new List<Detection>();
        }
    }

    private void Update()
    {
        //PushNewDetectionToQueue();
        CalculateGestureConfidence();
        DecideOnCurrentGesture();
    }

    private void DecideOnCurrentGesture()
    {
        if (gestureOverrideMode)
        {
            if (_currentGesture.gestureID != _lastGesture.gestureID)
            {
                _gestureIdReference.Value = _currentGesture.gestureID;
                _lastGesture = _currentGesture;
            }
            return;
        }
        _currentGesture = currentGestureScores.First(e => e.gestureID == _currentGesture.gestureID);
        //if this runs, it means we're actively trying to maintain a gesture, only discard below a threshold
        if (_currentGesture.confidence >= gestureMaintenanceReq)
        {
            return;
        }
        _currentGesture = new Detection(Gesture.GestureID.None, 0f);
        var highestConfidence = new Detection(Gesture.GestureID.None, 0f);
        //if this runs, no gesture is being made, so we look for the one with the highest score
        for (int i = 0; i < currentGestureScores.Count; i++)
        {
            if (currentGestureScores[i].confidence > highestConfidence.confidence)
            {
                highestConfidence = currentGestureScores[i];
            }
        }
        if (highestConfidence.confidence <= gestureInitializationReq)
        {
            _currentGesture.confidence = 0f;
            _gestureIdReference.Value = Gesture.GestureID.None;
            return;
        }
        _currentGesture = highestConfidence;
        _gestureIdReference.Value = _currentGesture.gestureID;
    }
    public void PushRemoteDetectionToQueue(float[] detections)
    {
        for (int i = 0; i < GestureIDList.Count; i++)
        {
            if (_detectionFrame[i].Count == framesToConsider)
            {
                _detectionFrame[i].RemoveAt(framesToConsider-1);
            }
            _detectionFrame[i].Insert(0,new Detection((Gesture.GestureID) i, detections[i]));

        }
    }
    private void PushNewDetectionToQueue()
    {
        for (int i = 1; i < GestureIDList.Count; i++)
        {
            if (_detectionFrame[i].Count == framesToConsider)
            {
                _detectionFrame[i].RemoveAt(framesToConsider-1);
            }
            _detectionFrame[i].Insert(0,new Detection((Gesture.GestureID) i, CycleConfidenceValues(i-1)));

        }
    }

    // ReSharper restore Unity.ExpensiveCode
    private float CycleConfidenceValues(int number)
    {
        return math.remap(-1f,1f,0f,1f,math.sin((Time.time - number) / GestureIDList.Count-2));
    }

    private void CalculateGestureConfidence()
    {
        currentGestureScores.Clear();
        for (int i = 0; i < GestureIDList.Count; i++)
        {
            var confidence = 0f;
            var temporalModifier = 0f;
            for (int j = 0; j < _detectionFrame[i].Count; j++)
            {
                var temporalWeight = confidenceWeighing.Evaluate((float)j / framesToConsider);
                temporalModifier += temporalWeight;
                confidence += _detectionFrame[i][j].confidence * temporalWeight;
            }
            temporalModifier = 1 / (temporalModifier / _detectionFrame[i].Count);
            confidence /= _detectionFrame[i].Count;
            confidence *= temporalModifier;
            
            currentGestureScores.Add(new Detection((Gesture.GestureID)i, confidence));
        }
        
    }

    
    
}
