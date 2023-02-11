using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

public class RotateInteractor2 : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _rotateAnchor;
    [SerializeField] private GestureIdReference _currentGesture;
    [SerializeField] private InteractableReference _currentSelectionRef;
    private GenericInteractable _currentSelection;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float _radius;
    [SerializeField] private GameObject widgetPrefab;
    [SerializeField] private LineRenderer directionRenderer;
    
    private GameObject spawnedWidget;
    private bool _gestureActive;
    private bool _trackingActive;
    private Transform _anchorTransform;
    private SphereCollider _nearInteractionCollider;
    private Collider[] _colliders = new Collider[3];
    private bool _nearInteraction;
    private bool _farInteractionWidgetSpawned;
    Vector3 directionVector;
    private float width;
    [SerializeField] private float minMagnitude = 0.5f;

    private void Start()
    {
        width = widgetPrefab.GetComponent<MeshRenderer>().bounds.size.x * 0.6f;

    }

    private void Update()
    {
        if (!_trackingActive || !rotationActive) { return; }
        if (_nearInteraction)
        {
            var processedPos = _anchorTransform.position - interactableTransform.position;
            processedPos.Normalize();
            _config.Process(false,ref processedPos);
            interactableTransform.rotation = Quaternion.LookRotation(processedPos);
        }
        else
        {
            if (!_farInteractionWidgetSpawned){ return;}
            directionVector = Vector3.ClampMagnitude(_anchorTransform.position - spawnedWidget.transform.position,width);
            _config.Process(true, ref directionVector);
            if (directionVector.magnitude < minMagnitude * width)
            {
                return;
            }
            directionRenderer.startColor = new Color(directionVector.magnitude, 1 - directionVector.magnitude, 0, 255);
            directionRenderer.endColor = new Color(directionVector.magnitude, 1 - directionVector.magnitude, 0, 255);
            var rot = Quaternion.LookRotation(directionVector.normalized);
            interactableTransform.rotation = rot;
            directionRenderer.SetPosition(1,spawnedWidget.transform.position + directionVector);
        }
    }

    private bool IsNearInteraction()
    {
        int count = Physics.OverlapSphereNonAlloc(_anchorTransform.position, _radius, _colliders, _interactableLayerMask,QueryTriggerInteraction.Collide);
        bool isNearby = false;
        for (int i = 0; i < count; i++)
        {
            if (_colliders[i].gameObject != _currentSelection.gameObject) continue;
            isNearby = true;
            break;
        }
        return isNearby;
    }

    #region EventRegistration
    private void OnEnable()
    {
        _rotateAnchor.RegisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.RegisterCallback(OnGestureChanged);
        _currentSelectionRef.RegisterCallback(OnSelectionChanged);
    }

    private void OnSelectionChanged(GenericInteractable genericInteractable)
    {
        if (_currentSelection == _currentSelectionRef.Value)
        {
            return;
        }
        if (_currentSelection != null)
        {
            _currentSelection.StopRotation();
        }
        _currentSelection = _currentSelectionRef.Value;
        ConditionalActivation();
    }

    private void OnDisable()
    {
        _rotateAnchor.UnregisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.UnregisterCallback(OnGestureChanged);
    }
    #endregion
    
    #region EventCallbacks

    private Transform interactableTransform;
    private RotationConfiguration _config;
    private bool rotationActive;
    private Vector3 widgetSpawnDirection;
    [SerializeField] private float rotateSpeed = 0.1f;
    private Vector3 rotationOffset;

    private void OnGestureChanged(Gesture.GestureID gestureID)
    {
        _gestureActive = _currentGesture == Gesture.GestureID.Rotate;
        ConditionalActivation();
    }

    private void ConditionalActivation()
    {
        if (!_gestureActive)
        {
            ToggleFarInteractionWidget(false);
            rotationActive = false;
            if (_currentSelection != null)
            {
                _currentSelection.StopRotation();
            }
            return;
        }
        if (_currentSelection == null)
        {
            return;
        }
        rotationActive = _currentSelection.VerifyOwnership(ref interactableTransform) || _currentSelection.StartRotation(ref _config, ref interactableTransform);
        if (!rotationActive)
        {
            return;
        }
        _nearInteraction = IsNearInteraction();
        if (_nearInteraction)
        {
            rotationOffset = interactableTransform.position - _anchorTransform.position;
            return;
        }

        if (_farInteractionWidgetSpawned)
        {
            return;
        }
        ToggleFarInteractionWidget(true);
    }

    private void ToggleFarInteractionWidget(bool toggle)
    {
        _farInteractionWidgetSpawned = toggle;
        directionRenderer.enabled = toggle;
        if (spawnedWidget != null)
        {
            Destroy(spawnedWidget);
        }
        if (toggle)
        {
            var position = _anchorTransform.position;
            spawnedWidget = Instantiate(widgetPrefab, position, Quaternion.identity);
            directionRenderer.SetPosition(0,position);
            directionRenderer.SetPosition(1,position);
        }
    }
    
    private void OnAnchorCreate(Object obj)
    {
        _anchorTransform = ((GameObject)obj).transform;
        _trackingActive = true;
    }
    private void OnAnchorDestroy(Object obj)
    {
        _anchorTransform = null;
        _trackingActive = false;
    }
    #endregion
}