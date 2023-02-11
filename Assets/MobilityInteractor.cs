using System;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MobilityInteractor : GenericInteractor
{
    [SerializeField] private ObjectReference playerCameraReference;
    
    [SerializeField] private ObjectReference _worldReference;
    [SerializeField] private float _scalePerUnityMetre = 1f;
    [SerializeField] private float _movementPerUnityMetre = 1f;
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField,Min(0.01f)] private float _minScale = 0.01f;
    [SerializeField,Min(0.01f)] private float _maxScale = 100f;
    [SerializeField] private GameObject originWidgetPrefab;
    [SerializeField] private GameObject targetWidgetPrefab;
    [SerializeField] private Material maxedOut;
    [SerializeField] private float safetyZone;
    
    private GameObject spawnedOriginWidget,spawnedTargetWidget;

    private Vector3 _minScaleCalc, _maxScaleCalc;
    private Transform _worldTransform;
    private Vector3 _initialMidPoint, _currentMidpoint;
    private float _initialDistance, _currentDistance, _percentageFromInitialDist;
    private Vector3 _initialPosition, _initialScale;
    private Vector3 currentPositionalDifference;
    private LineRenderer _lineRenderer;
    private Transform playerTransform;
    private bool locallyActive;
    private MeshRenderer spawnedTargetWidgetMeshRenderer;
    private Color defaultColor;
    private void Start()
    {
        _worldTransform = ((GameObject)_worldReference.Value).transform;
        playerTransform = ((GameObject)playerCameraReference).transform;
        _minScaleCalc = Vector3.one * _minScale;
        _maxScaleCalc = Vector3.one * _maxScale;
        spawnedOriginWidget = Instantiate(originWidgetPrefab);
        spawnedTargetWidget = Instantiate(targetWidgetPrefab);
        spawnedOriginWidget.SetActive(false);
        spawnedTargetWidget.SetActive(false);
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        spawnedTargetWidgetMeshRenderer = spawnedTargetWidget.GetComponent<MeshRenderer>();
        defaultColor = spawnedTargetWidgetMeshRenderer.material.color;
    }

    private void OnValidate()
    {
        _handedness = Handedness.Both;
    }

    private Vector3 lastLeftHandPosition, lastRightHandPosition;
    private void Update()
    {
        if (_interactorActive && locallyActive)
        {
            if (Vector3.Distance(lastLeftHandPosition, _leftHandTransform.position) > safetyZone || Vector3.Distance(lastRightHandPosition, _rightHandTransform.position) > safetyZone)
            {
                return;
            }

            lastLeftHandPosition = _leftHandTransform.position;
            lastRightHandPosition = _rightHandTransform.position;
            _currentDistance = Vector3.Distance(_leftHandTransform.position, _rightHandTransform.position);
            _currentMidpoint = Vector3.Lerp(_leftHandTransform.position, _rightHandTransform.position, 0.5f);
            _percentageFromInitialDist = _currentDistance / _initialDistance;
            var newscale = _scalePerUnityMetre * _percentageFromInitialDist * _initialScale;
            if (float.IsNaN(newscale.x) || float.IsNaN(newscale.y) || float.IsNaN(newscale.z))
            {
                newscale = _worldTransform.localScale;
            }
            newscale.x = Math.Clamp(newscale.x, _minScaleCalc.x, _maxScaleCalc.x);
            newscale.y = Math.Clamp(newscale.y, _minScaleCalc.y, _maxScaleCalc.y);
            newscale.z = Math.Clamp(newscale.z, _minScaleCalc.z, _maxScaleCalc.z);

            _worldTransform.ScaleAround(_currentMidpoint, newscale);
            if (newscale == _minScaleCalc || newscale == _maxScaleCalc)
            {
                spawnedTargetWidgetMeshRenderer.material.color = Color.red;
            }
            else
            {
                spawnedTargetWidgetMeshRenderer.material.color = defaultColor;
            }
            spawnedTargetWidget.transform.position = _currentMidpoint;
            playerTransform.position = _initialPosition + _movementPerUnityMetre * (_currentMidpoint - _initialMidPoint);

            //_lineRenderer.SetPosition(1,_currentMidpoint + _movementPerUnityMetre * (_currentMidpoint - _initialMidPoint));
        }
    }

    
    private protected override void OnInteractorDisable()
    {
        locallyActive = false;
        spawnedOriginWidget.SetActive(false);
        spawnedTargetWidget.SetActive(false);
        _lineRenderer.enabled = false;
    }

    private protected override void OnInteractorEnable()
    {
        locallyActive = true;
        lastLeftHandPosition = _leftHandTransform.position;
        lastRightHandPosition = _rightHandTransform.position;
        _initialDistance = Math.Clamp(Vector3.Distance(_leftHandTransform.position, _rightHandTransform.position),0.1f,2f);
        _initialMidPoint = Vector3.Lerp(_leftHandTransform.position, _rightHandTransform.position, 0.5f);
        _initialPosition = playerTransform.position;
        _initialScale = _worldTransform.localScale;
        
        //spawnedOriginWidget.SetActive(true);
        spawnedTargetWidget.SetActive(true);
        //spawnedOriginWidget.transform.position = _initialMidPoint;
        spawnedTargetWidget.transform.position = _initialMidPoint;

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0,_initialMidPoint);
        _lineRenderer.SetPosition(1,_initialMidPoint);

    }
}