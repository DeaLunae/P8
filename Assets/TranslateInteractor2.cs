using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class TranslateInteractor2 : GenericInteractor
{
    [SerializeField] private InteractableReference _currentSelectionRef;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float translationSpeed = 1f;
    
    private GenericInteractable _currentSelection;
    private Collider[] _colliders = new Collider[3];
    private bool locallyActive;
    private Transform interactableTransform;
    private TranslationConfiguration translationConfiguration;
    private Vector3 translationOffset;
    private Quaternion interactableRotationOffset;
    private Quaternion interactorRotationOffset;

    void Update()
    {
        if (_interactorActive && locallyActive)
        {
            var processedPos = FindAppropriateTransform().position;
            translationConfiguration.Process(false,ref processedPos);
            interactableTransform.position = Vector3.MoveTowards(interactableTransform.transform.position,
                processedPos + translationOffset, Time.deltaTime * translationSpeed);
            interactableTransform.rotation = FindAppropriateTransform().rotation * Quaternion.Inverse(interactorRotationOffset) * interactableRotationOffset;
        }
    }
    private bool FindNearestSelectable()
    {
        var pos = FindAppropriateTransform().position;
        int count = Physics.OverlapSphereNonAlloc(pos, _radius, _colliders, _interactableLayerMask,QueryTriggerInteraction.Collide);
        float dist = float.PositiveInfinity;
        GenericInteractable genericInteractable;
        GenericInteractable closest = null;
        for (int i = 0; i < count; i++)
        {
            genericInteractable = _colliders[i].GetComponent<GenericInteractable>();
            if (genericInteractable == null) { continue; }
            switch (_editingStateReference.Value)
            {
                case EditingState.Spline when !genericInteractable.gameObject.CompareTag("Spline"):
                case EditingState.Object when !genericInteractable.gameObject.CompareTag("Object"):
                    continue;
            }
            if (Vector3.Distance(pos, genericInteractable.gameObject.transform.position) >= dist) {continue;}
            dist = Vector3.Distance(pos, genericInteractable.gameObject.transform.position);
            closest = genericInteractable;
        }

        if (closest == null) return false;
        if (_currentSelectionRef.Value != null)
        {
            _currentSelectionRef.Value.StopSelect();
        }
        _currentSelectionRef.Value = closest;
        _currentSelection = closest;
        return _currentSelectionRef.Value.StartSelect();

    }

    private Transform FindAppropriateTransform()
    {
        return _handedness switch
        {
            Handedness.Left => _leftHandTransform,
            Handedness.Right => _rightHandTransform,
            _ => null
        };
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _currentSelectionRef.RegisterCallback(OnSelectionChanged);
    }

    private void OnSelectionChanged(GenericInteractable genericInteractable)
    {
        if (_currentSelection != null)
        {
            if (_currentSelection == _currentSelectionRef.Value)
            {
                return;
            }
            _currentSelection.StopTranslation();
        }
        _currentSelection = _currentSelectionRef.Value;
    }

    private protected override void OnInteractorDisable()
    {
        locallyActive = false;
        if (_currentSelection != null)
        {
            _currentSelection.StopTranslation();
        }
    }

    private protected override void OnInteractorEnable()
    {
        locallyActive = FindNearestSelectable();
        if (locallyActive)
        {
            _currentSelection.StartTranslation(ref translationConfiguration,ref interactableTransform);
            translationOffset = interactableTransform.position - FindAppropriateTransform().position;
            interactorRotationOffset = FindAppropriateTransform().rotation;
            interactableRotationOffset = interactableTransform.rotation;
            
        }
    }
}
