using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.VisualScripting;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

public class DemonstrationPoseRecorder : MonoBehaviour
{
    [SerializeField] private Material handMaterial;
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    private Gesture currentGesture;

    private void Start()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected += GestureSelected;
            _gestureRecorderEvents.OnStartRecordingDemonstrationGesture += RecordDemonstrationGesture;
        }
    }

    private void RecordDemonstrationGesture()
    {
        CaptureHand(currentGesture);
    }

    private void OnDestroy()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
        }
    }
    private void GestureSelected(object obj)
    {
        currentGesture = (Gesture)obj;
    }

    public void CaptureHand(Gesture gesture)
    {
        var rhvs = FindObjectsOfType<RiggedHandVisualizer>();
        var correctSmr = rhvs.First(e =>
            e.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.name
                .StartsWith(gesture.handedness == Handedness.Right ? "R" : "L"));
        var path = AssetDatabase.GetAssetPath(gesture).Split("/");
        var folderpath = "";
        for (int i = 0; i < path.Length - 1; i++)
        {
            folderpath += path[i] + "/";
        }

        folderpath +=
            $"{gesture.id.ToString()}-{gesture.handedness.ToString()}-{Directory.EnumerateFiles(folderpath).Count(e => e.EndsWith(".prefab")) + 1}.prefab";

        correctSmr.transform.parent.gameObject.SetActive(false);
        var GameObjectIWantToSave = Instantiate(correctSmr.transform.parent.gameObject);
        correctSmr.transform.parent.gameObject.SetActive(true);
        GameObjectIWantToSave.GetComponentInChildren<Animator>();
        GameObjectIWantToSave.GetComponentInChildren<ObjectEvent>();
        GameObjectIWantToSave.GetComponentInChildren<SkinnedMeshRenderer>().material = handMaterial;
        var wrist = GameObjectIWantToSave.GetComponentsInChildren<Transform>().First(e => e.gameObject.name.Contains("Wrist"));
        wrist.transform.position = Vector3.zero;
        wrist.transform.position = GetCenterPosition(wrist);
        var go = PrefabUtility.SaveAsPrefabAsset(GameObjectIWantToSave, folderpath);
        Destroy(GameObjectIWantToSave);
        gesture.HandPrefab = go;
        _gestureRecorderEvents.DoneRecordingDemonstrationGesture();
    }

    private Vector3 GetCenterPosition(Transform t)
    {
        var children = t.GetComponentsInChildren<Transform>();
        var center = children.Aggregate(Vector3.zero, (current, t) => current + t.position);
        return center / (children.Length);
    }
}
#endif
