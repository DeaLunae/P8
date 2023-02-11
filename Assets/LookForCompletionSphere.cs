using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LookForCompletionSphere : MonoBehaviour
{
    [SerializeField] private InteractableTaskReference currentTask;
    [SerializeField] private LayerMask _layerMask;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void Look()
    {
        var tci = CheckIfCompletionSphereNearby();
        if (tci != null)
        {
            if (tci.task.Value.index == currentTask.Value.index)
            {
                tci.OnTaskCompleted();
            }
        }
    }
    
    public TaskCompletionInvoker CheckIfCompletionSphereNearby()
    {
        var average = _collider.bounds.size.x + _collider.bounds.size.y + _collider.bounds.size.z;
        average = (average / 3) * 1.2f;
        var colliders = Physics.OverlapSphere(gameObject.transform.position, average, _layerMask,QueryTriggerInteraction.Collide);
        if (colliders.Length == 0)
        {
            return null;
        }
        colliders = colliders.Where(e => e.gameObject.CompareTag("Task")).ToArray();
        if (colliders.Length == 0)
        {
            return null;
        }
        var result = colliders.OrderBy(e => Vector3.Distance(gameObject.transform.position, e.transform.position)).FirstOrDefault();
        return result == null ? null : result.gameObject.GetComponent<TaskCompletionInvoker>();
    }
}
