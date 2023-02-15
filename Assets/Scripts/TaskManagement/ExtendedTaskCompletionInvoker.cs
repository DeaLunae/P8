using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedTaskCompletionInvoker : MonoBehaviour
{
    [SerializeField] public List<InteractableTaskReference> tasks;
    public void OnTaskCompleted()
    {
        foreach (var task in tasks)
        {
            task.Value = task.Value;
        }
    }
}
