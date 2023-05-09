using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class KeyboardHighlightResponder : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TMP_Text keyText;
    private void CheckHighlightStatus()
    {
        meshRenderer.material = CompareKeyToResponse() ? highlightMaterial : defaultMaterial;
    }
    
    private bool CompareKeyToResponse()
    {
        return gestureRecorderEvents.endOfListReached ? CheckIfCurrentGesture() : gestureRecorderEvents.CurrentGesture.UserSessionResponse.ToString().Replace("Nummer", "") == keyText.text;
    }
    private void OnEnable()
    {
        gestureRecorderEvents.OnKeyboardButtonPressed += CheckHighlightStatus;
        gestureRecorderEvents.OnGestureSelected += CheckHighlightStatus;
        gestureRecorderEvents.EndOfListReached += DisableKey;
    }

    private bool CheckIfCurrentGesture()
    {
        Enum.TryParse(int.TryParse(keyText.text, out _) ? $"Nummer{keyText.text}" : keyText.text, out Gesture.GestureName gestureName);
        return gestureRecorderEvents.CurrentGesture.Name == gestureName;
    }
    private void DisableKey()
    {
        Enum.TryParse(int.TryParse(keyText.text, out _) ? $"Nummer{keyText.text}" : keyText.text, out Gesture.GestureName gestureName);
        if (gestureRecorderEvents.wrongGestures.All(e => e.Name != gestureName))
        {
            gameObject.SetActive(false);
        }
    }

    private void CheckHighlightStatus(object obj)
    {
        CheckHighlightStatus();
    }

    private void OnDisable()
    {
        gestureRecorderEvents.OnKeyboardButtonPressed -= CheckHighlightStatus;

        gestureRecorderEvents.OnKeyboardButtonPressed -= CheckHighlightStatus;
        gestureRecorderEvents.OnGestureSelected -= CheckHighlightStatus;
    }

    private void OnDestroy()
    {
        gestureRecorderEvents.OnKeyboardButtonPressed -= CheckHighlightStatus;

        gestureRecorderEvents.OnKeyboardButtonPressed -= CheckHighlightStatus;
        gestureRecorderEvents.OnGestureSelected -= CheckHighlightStatus;
    }
}
