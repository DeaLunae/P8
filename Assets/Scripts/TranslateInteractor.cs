using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

public class TranslateInteractor : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _translateAnchor;
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
    private void Start()
    {
        width = widgetPrefab.GetComponent<MeshRenderer>().bounds.size.x * 0.6f;
    }

    private void Update()
    {
        if (!_trackingActive || !translationActive) { return; }
        if (_nearInteraction)
        {
            var processedPos = _anchorTransform.position;
            _config.Process(false,ref processedPos);
            interactableTransform.position = Vector3.MoveTowards(interactableTransform.transform.position,processedPos+translationOffset,0.4f);
        }
        else
        {
            if (!_farInteractionWidgetSpawned){ return;}
            directionVector = Vector3.ClampMagnitude(_anchorTransform.position - spawnedWidget.transform.position,1f);

            directionRenderer.startColor = new Color(directionVector.magnitude, 1 - directionVector.magnitude, 0, 255);
            directionRenderer.endColor = new Color(directionVector.magnitude, 1 - directionVector.magnitude, 0, 255);

            _config.Process(true,ref directionVector);
            interactableTransform.Translate(directionVector * (Time.deltaTime * translateSpeed),Space.World);
            directionVector = Vector3.ClampMagnitude(directionVector, width);

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
        _translateAnchor.RegisterListener(OnAnchorCreate, OnAnchorDestroy);
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
            _currentSelection.StopTranslation();
        }
        _currentSelection = _currentSelectionRef.Value;
        ConditionalActivation();
    }

    private void OnDisable()
    {
        _translateAnchor.UnregisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.UnregisterCallback(OnGestureChanged);
    }
    #endregion
    
    #region EventCallbacks

    private Transform interactableTransform;
    private TranslationConfiguration _config;
    private bool translationActive;
    private Vector3 widgetSpawnDirection;
    [SerializeField] private float translateSpeed = 0.1f;
    private Vector3 translationOffset;

    private void OnGestureChanged(Gesture.GestureID gestureID)
    {
        _gestureActive = _currentGesture == Gesture.GestureID.Translate;
        ConditionalActivation();
    }

    private void ConditionalActivation()
    {
        if (!_gestureActive)
        {
            ToggleFarInteractionWidget(false);
            translationActive = false;
            if (_currentSelection != null)
            {
                _currentSelection.StopTranslation();
            }
            return;
        }
        if (_currentSelection == null)
        {
            return;
        }
        translationActive = _currentSelection.VerifyOwnership(ref interactableTransform) || _currentSelection.StartTranslation(ref _config, ref interactableTransform);
        if (!translationActive)
        {
            return;
        }
        _nearInteraction = IsNearInteraction();
        if (_nearInteraction)
        {
            translationOffset = interactableTransform.position - _anchorTransform.position;
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