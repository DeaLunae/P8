using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

/*
public class RotateInteractor : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _rotateAnchor;
    [SerializeField] private GestureIdReference _currentGesture;
    [SerializeField] private InteractableReference _currentSelection;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float _radius;
    [SerializeField] private GameObject _widgetPrefab;
    [SerializeField] private LineRenderer _directionRenderer;
    
    private GameObject _spawnedWidget;
    private bool _gestureActive;
    private bool _trackingActive;
    private Transform _anchorTransform;
    private SphereCollider _nearInteractionCollider;
    private Collider[] _colliders = new Collider[3];
    private bool _nearInteraction;
    private bool _farInteractionWidgetSpawned;
    
    Vector3 _directionVector;
    private float _width;
    private void Start()
    {
        _width = _widgetPrefab.GetComponent<MeshRenderer>().bounds.extents.x;
    }

    private void Update()
    {
        if (!_trackingActive || !_rotationActive) { return; }
        if (_nearInteraction)
        {
            var processedPos = _anchorTransform.position - _interactableTransform.position;
            processedPos.Normalize();
            _config.Process(false,ref processedPos);
            _interactableTransform.rotation = Quaternion.LookRotation(processedPos);
        }
        else
        {
            if (!_farInteractionWidgetSpawned){ return;}
            _directionVector = Vector3.ClampMagnitude(_anchorTransform.position - _spawnedWidget.transform.position,_width);
            _config.Process(true, ref _directionVector);
            if (_directionVector.magnitude < minMagnitude)
            {
                return;
            }
            _directionRenderer.startColor = new Color(_directionVector.magnitude, 1 - _directionVector.magnitude, 0, 255);
            _directionRenderer.endColor = new Color(_directionVector.magnitude, 1 - _directionVector.magnitude, 0, 255);
            var rot = Quaternion.LookRotation(_directionVector.normalized);
            _interactableTransform.rotation = rot;
            _directionRenderer.SetPosition(1,_spawnedWidget.transform.position + _directionVector);
        }
    }

    private bool IsNearInteraction()
    {
        int count = Physics.OverlapSphereNonAlloc(_anchorTransform.position, _radius, _colliders, _interactableLayerMask,QueryTriggerInteraction.Collide);
        bool isNearby = false;
        for (int i = 0; i < count; i++)
        {
            if (_colliders[i].gameObject != _currentSelection.Value.gameObject) continue;
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
        _currentSelection.RegisterCallback(OnSelectionChanged);
    }

    private void OnSelectionChanged()
    {
        ToggleFarInteractionWidget(false);
    }

    private void OnDisable()
    {
        _rotateAnchor.UnregisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.UnregisterCallback(OnGestureChanged);
        _currentSelection.UnregisterCallback(OnSelectionChanged);

    }
    #endregion
    
    #region EventCallbacks
    
    private Transform _interactableTransform;
    private RotationConfiguration _config;
    private bool _rotationActive;
    private Vector3 _widgetSpawnDirection;
    [SerializeField] private float _rotateSpeed = 0.1f;
    
    private void OnGestureChanged()
    {
        _gestureActive = _currentGesture == Gesture.GestureID.Rotate;
        _rotationActive = false;
        if (!_gestureActive)
        {
            ToggleFarInteractionWidget(false);
            return;
        }
        if (_currentSelection.Value == null)
        {
            return;
        }
        _rotationActive = _currentSelection.Value.StartRotation(ref _config, ref _interactableTransform);
       
        _nearInteraction = IsNearInteraction();
        if (_nearInteraction) { return; }
        ToggleFarInteractionWidget(true);
    }

    private void ToggleFarInteractionWidget(bool toggle)
    {
        _farInteractionWidgetSpawned = toggle;
        _directionRenderer.enabled = toggle;
        if (_spawnedWidget != null)
        {
            Destroy(_spawnedWidget);
        }
        if (toggle)
        {
            var position = _anchorTransform.position;
            /*_widgetSpawnDirection = CameraCache.Main.transform.rotation.eulerAngles;
            _widgetSpawnDirection.z = 0;
            _widgetSpawnDirection.x = 0;#1#
            
            _spawnedWidget = Instantiate(_widgetPrefab, position, quaternion.identity);
            _directionRenderer.SetPosition(0, position);
            _directionRenderer.SetPosition(1, position);
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
}*/