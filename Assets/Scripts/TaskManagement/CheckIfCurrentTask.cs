using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckIfCurrentTask : MonoBehaviour
{
    [SerializeField] private InteractableTaskReference currentTask;
    [SerializeField] private InteractableTaskReference thisTask;
    private Renderer _renderer;
    private Collider _collider;
    private void OnEnable()
    {
        currentTask.RegisterCallback(OnTaskChanged);
    }
    private void OnDisable()
    {
        currentTask.UnregisterCallback(OnTaskChanged);
    }
    private void OnTaskChanged(InteractableTask interactableTask)
    {
        if (currentTask.Value.index == thisTask.Value.index)
        {
            _renderer.enabled = true;
            _collider.enabled = true;
        }
        else
        {

            _renderer.enabled = false;
            _collider.enabled = false;
            
        }
    }
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }
}
