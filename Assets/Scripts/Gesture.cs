using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

[CreateAssetMenu]

public class Gesture : ScriptableObject
{
    public GestureID id;
    [Min(1)] public int amountOfSampleFrames = 600;
    public Handedness handedness;
    public TextAsset file;
    [SerializeField] private GameObject handPrefab;

    public GameObject HandPrefab
    {
        get => handPrefab;
        set
        {
            SetDirty();
            handPrefab = value;
        }
    }
    
    private new void SetDirty()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
    [Serializable]
    public enum GestureID
    {
        None,
        Translate,
        Rotate,
        Scale,
        Hover,
        Select,
        TeleportHover,
        TeleportSelect
    }
}
