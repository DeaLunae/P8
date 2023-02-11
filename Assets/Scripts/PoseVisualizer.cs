using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

public class PoseVisualizer : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Transform _anchorTransform;
    private GameObject spawnedPose;

    private Gesture currentGesture;
    // Start is called before the first frame update
    void Start()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.OnGestureSelected += GestureSelected;
            gestureRecorderEvents.OnStartRecordingDemonstrationGesture += RemoveOldPose;
            gestureRecorderEvents.OnDoneRecordingDemonstrationGesture += DisplayUpdatedPose;
        }
    }

    private void RemoveOldPose()
    {
        if (spawnedPose != null)
        {
            Destroy(spawnedPose);
        }
    }

    private void DisplayUpdatedPose()
    {
        if (currentGesture != null && currentGesture.HandPrefab != null)
        {
            spawnedPose = Instantiate(currentGesture.HandPrefab, parent: _anchorTransform);
            spawnedPose.transform.position = _anchorTransform.position - GetCenterPosition(currentGesture.HandPrefab.GetComponentsInChildren<Transform>());
        }
    }
    private Vector3 GetCenterPosition(IReadOnlyCollection<Transform> transforms)
    {
        return transforms.Aggregate(Vector3.zero, (curr, t) => curr + t.position) / transforms.Count;
    }
    private void GestureSelected(object obj)
    {
        currentGesture = (Gesture)obj;
        RemoveOldPose();
        DisplayUpdatedPose();
    }

    private void OnDestroy()
    {
        if (gestureRecorderEvents != null)
        {
            gestureRecorderEvents.OnGestureSelected -= GestureSelected;
            gestureRecorderEvents.OnStartRecordingDemonstrationGesture -= RemoveOldPose;
            gestureRecorderEvents.OnDoneRecordingDemonstrationGesture -= DisplayUpdatedPose;
        }
    }
}
