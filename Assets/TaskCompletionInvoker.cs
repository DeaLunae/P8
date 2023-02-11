using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Devkit.Modularis.Variables;
using Google.Protobuf.WellKnownTypes;
using UnityEditor;
using UnityEngine;


public class TaskCompletionInvoker : MonoBehaviour
{
    [SerializeField] public InteractableTaskReference task;
    public void OnTaskCompleted()
    {
        task.Value = task.Value;
    }
}
