using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using TMPro;
using UnityEngine;

public class UpdateTaskText : MonoBehaviour
{
    
    [SerializeField] private TextMeshPro currentTaskText;
    [SerializeField] private InteractableTaskReference currentReferenceDescription;
    private void OnEnable()
    {
        currentReferenceDescription.RegisterCallback(OnTaskUpdated);
        currentTaskText.SetText(currentReferenceDescription.Value.taskDescription);
    }
    private void OnDisable()
    {
        currentReferenceDescription.UnregisterCallback(OnTaskUpdated);
    }
    private void OnTaskUpdated(InteractableTask interactableTask)
    {
        currentTaskText.SetText(currentReferenceDescription.Value.taskDescription);
        currentTaskText.ForceMeshUpdate();
    }
}
