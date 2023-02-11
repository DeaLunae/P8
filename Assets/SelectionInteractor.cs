using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devkit.Modularis.Events;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;

[RequireComponent(typeof(LineRenderer))]
public class SelectionInteractor : MonoBehaviour
{
    [SerializeField] private InteractableReference _currentlySelectedInteractable;
    [SerializeField] private ObjectEventReference _originPoint;
    [SerializeField] private GestureIdReference _currentGesture;
    [SerializeField,UnityEngine.Min(1f)] private float _maxRadius = 1f;
    [SerializeField,UnityEngine.Min(1f)] private float _maxDistance = 1f;
    [SerializeField,UnityEngine.Min(1f)] private float _coneAngle = 1f;
    [SerializeField] private LayerMask _selectableLayerMask;
    [SerializeField,UnityEngine.Min(3)] private int _lineResolution = 100;
    [SerializeField,Range(0.01f,1f)] private float _bezierCurveDistance;
    [SerializeField] private int _directionSmoothingFrameCount;
    [SerializeField] private float _gestureForgivenessDelay;
    #region Variables
    private Transform _op;
    private LineRenderer _lineRenderer;
    private GenericInteractable _currHoverInter = null;
    private GenericInteractable _newHoverTargetInteractable;
    private RaycastHit _newHoverTargetRaycast;
    private Vector3 _p0, _p1, _p2;
    private float _t = 0f;
    private Vector3[] _linePositions;
    private readonly List<Vector3> _directions = new List<Vector3>();
    private Vector3 _smoothedDirection;
    private RaycastHit[] _sphereCastHits;
    private RaycastHit _currHit;
    private int _sphereCastMaxHitCount = 10;
    private bool _visualsEnabled;
    private const int SphereCastLimit = 100;
    private GenericInteractable _coneCastGenericInteractable;
    private bool _trackingActive;
    private bool _selectEnabled;

    #endregion
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _linePositions = new Vector3[_lineResolution];
        _lineRenderer.enabled = false;
    }
    private void LateUpdate()
    {
        
        if (_visualsEnabled && _trackingActive)
        {
            if (LookForNewTarget())
            {
                _lineRenderer.enabled = true;
                GenerateBezierLineRenderer();
                
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }
    public bool LookForNewTarget()
    {
        
        var target = ConeCastWithChecks(_op.position, _op.right, _maxRadius, _maxDistance, _coneAngle, _selectableLayerMask, .2f, 0.3f, 0.2f, 0.3f, out _newHoverTargetRaycast);
        if (_currHoverInter != null)
        {
            _currHoverInter.StopHover();
        }
        if (!target)
        {
            print("FAIL: No hover target found");
            
            return false;
        }

        if (!_newHoverTargetRaycast.transform.gameObject.TryGetComponent(out _newHoverTargetInteractable))
        {
            print($"FAIL: GenericInteractable doesn't exist on found hover target named{_newHoverTargetRaycast.transform.gameObject.name}");
            return false;
        }

        if (!_newHoverTargetInteractable.StartHover())
        {
            print($"Target was {(_newHoverTargetInteractable.Hovered() ? "Already being hovered" : "Not hoverable")}");
            return false;
        }
        print("SUCCESS: GenericInteractable was found and can be hovered");

        _currHoverInter = _newHoverTargetInteractable;
        return true;
    }
    public void GenerateBezierLineRenderer()
    {
        if (_directions.Count > _directionSmoothingFrameCount)
        {
            _directions.RemoveAt(0);
        }
        _directions.Add(_op.right);
        _smoothedDirection = _directions.Aggregate(new Vector3(0,0,0), (s,v) => s + v).normalized;
        _p0 = _op.position;
        _p2 = _currHoverInter.transform.position;
        _p1 = FindNearestPointOnLine(_p0, _smoothedDirection, _p2);
        _p1 = Vector3.MoveTowards(_p0,_p1, _bezierCurveDistance*Vector3.Distance(_p0,_p1));
        for (int i = 0; i < _lineResolution; i++)
        {
            //B(t) = (1-t)^2*P0 + 2*(1-t)*t*P1 + t^2P2, 0 < t < 1
            _t = (float) i / _lineResolution;
            _linePositions[i] = math.pow(1f - _t, 2f) * _p0 + 2f * (1f - _t) * _t * _p1 + math.pow(_t, 2f) * _p2;
        }
        _lineRenderer.SetPositions(_linePositions);
    }
    #region Conecasting
    /// <summary>
    /// Casts a sphere along a ray and checks if the hitpoint is within the angle of the cone and returns the best target determined by the weights provided.
    /// </summary>
    /// <param name="raycastHit"></param>
    /// <param name="origin">The vertex of the cone and the at the start of the sweep.</param>
    /// <param name="direction">The direction into which to sweep the sphere..</param>
    /// <param name="maxRadius">The radius of the sweep.</param>
    /// <param name="maxDistance">The max length of the cast.</param>
    /// <param name="coneAngle">The angle used to define the cone.</param>
    /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
    /// <param name="distanceWeight">The importance of distance between the hitpoint and the origin in selecting the best target.</param>
    /// <param name="angleWeight">The importance of angle between the hitpoint and the origin in selecting the best target.</param>
    /// <param name="distanceToCenterWeight">The importance of distance between the hitpoint and the center of the object in selecting the best target.</param>
    /// <param name="angleToCenterWeight">The importance of angle between the hitpoint and the center of the object in selecting the best target.</param>
    /// <returns>The RaycastHit of the best object.</returns>
    public bool ConeCastWithChecks(Vector3 origin, Vector3 direction, float maxRadius, float maxDistance, float coneAngle, LayerMask layerMask, float distanceWeight, float angleWeight, float distanceToCenterWeight, float angleToCenterWeight,out RaycastHit raycastHit)
    {
        if (_sphereCastHits == null || _sphereCastHits.Length < _sphereCastMaxHitCount)
        {
            _sphereCastHits = new RaycastHit[_sphereCastMaxHitCount];
        }
        
        var hitCount = Physics.SphereCastNonAlloc(origin - (direction * maxRadius), maxRadius, direction, _sphereCastHits, maxDistance, layerMask, QueryTriggerInteraction.Collide);

        // Algorithm: double the max hit count if there are too many results, up to a certain limit
        if (hitCount >= _sphereCastMaxHitCount && _sphereCastMaxHitCount < SphereCastLimit)
        {
            // There might be more hits we didn't get, grow the array and try again next time
            // Note that this frame, the results might be imprecise.
            _sphereCastMaxHitCount = Math.Min(SphereCastLimit, _sphereCastMaxHitCount * 2);
        }
        raycastHit = new RaycastHit();
        float score = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            _currHit = _sphereCastHits[i];
            if (!_currHit.transform.TryGetComponent(out _coneCastGenericInteractable))
            {
                print("SEARCH: GenericInteractable not found in raycast");
                continue;
            }
            /*if (!_coneCastGenericInteractable.Hoverable())
            {
                print("SEARCH: GenericInteractable is not hoverable");
                continue;
            }*/

            Vector3 hitPoint = _currHit.point;
            Vector3 directionToHit = hitPoint - origin;
            float angleToHit = Vector3.Angle(direction, directionToHit);
            var position = _currHit.collider.transform.position;
            Vector3 hitDistance = position - hitPoint;
            Vector3 directionToCenter = position - origin;
            float angleToCenter = Vector3.Angle(direction, directionToCenter);

            // Additional work to see if there is a better point slightly further ahead on the direction line. This is only allowed if the collider isn't a mesh collider.
            if (_currHit.collider.GetType() != typeof(MeshCollider))
            {
                Vector3 pointFurtherAlongGazePath = (maxRadius * 0.5f * direction.normalized) + FindNearestPointOnLine(origin, direction, hitPoint);
                Vector3 closestPointToPointFurtherAlongGazePath = _currHit.collider.ClosestPoint(pointFurtherAlongGazePath);
                Vector3 directionToSecondaryPoint = closestPointToPointFurtherAlongGazePath - origin;
                float angleToSecondaryPoint = Vector3.Angle(direction, directionToSecondaryPoint);

                if (angleToSecondaryPoint < angleToHit)
                {
                    hitPoint = closestPointToPointFurtherAlongGazePath;
                    directionToHit = directionToSecondaryPoint;
                    angleToHit = angleToSecondaryPoint;
                    hitDistance = _currHit.collider.transform.position - hitPoint;
                }
            }

            if (!(angleToHit < coneAngle)) continue;
            float distanceScore = distanceWeight == 0 ? 0.0f : (distanceWeight * directionToHit.magnitude);
            float angleScore = angleWeight == 0 ? 0.0f : (angleWeight * angleToHit);
            float centerScore = distanceToCenterWeight == 0 ? 0.0f : (distanceToCenterWeight * hitDistance.magnitude);
            float centerAngleScore = angleToCenterWeight == 0 ? 0.0f : (angleToCenterWeight * angleToCenter);
            float newScore = distanceScore + angleScore + centerScore + centerAngleScore;

            if (!(newScore < score)) continue;
            score = newScore;
            raycastHit = _currHit;
        }
        return raycastHit.transform != null;
    }

    private static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        float dotP = Vector3.Dot(point - origin, direction);
        return origin + direction * dotP;
    }
    #endregion

    #region EventRegistration
    private void OnEnable()
    {
        _originPoint.RegisterListener(OnOriginCreate, OnOriginDestroy);
        _currentGesture.RegisterCallback(OnGestureChanged);
    }
    private void OnDisable()
    {
        _originPoint.UnregisterListener(OnOriginCreate, OnOriginDestroy);
        _currentGesture.UnregisterCallback(OnGestureChanged);
    }
    #endregion
    
    #region EventCallbacks
    private void OnGestureChanged(Gesture.GestureID gestureID)
    {
        if (_currentGesture == Gesture.GestureID.None)
        {
            if (_visualsEnabled)
            {
                Invoke(nameof(DelayHoverToggle), _gestureForgivenessDelay);
            }
        }
        else
        {
            _visualsEnabled = _currentGesture == Gesture.GestureID.Hover || _currentGesture == Gesture.GestureID.Select;
        }
        if (_currentGesture == Gesture.GestureID.Select)
        {
            SelectCurrentlyHoveredInteractable();
        }
    }

    private void DelayHoverToggle()
    {
        _visualsEnabled = _currentGesture == Gesture.GestureID.Hover || _currentGesture == Gesture.GestureID.Select;
        if (_visualsEnabled)
        {
            _lineRenderer.enabled = true;
        }
        else
        {
            _lineRenderer.enabled = false;
            if (_newHoverTargetInteractable != null)
            {
                _newHoverTargetInteractable.StopHover();
            }
        }
    }

    private void SelectCurrentlyHoveredInteractable()
    {
        //Exit clause: If no current hover target
        if (_currHoverInter == null) { return; }
        //Exit clause: If hover target is already the selected target;
        if (_currentlySelectedInteractable.Value == _currHoverInter) { return; }
        //Exit clause: if the currently selected isn't the same as the hovered, but the hovered isn't selectable
        if (!_currHoverInter.StartSelect()) { return; }
        
        if (_currentlySelectedInteractable.Value != null)
        {
            _currentlySelectedInteractable.Value.StopSelect();
        }
        _currentlySelectedInteractable.Value = _currHoverInter;
    }
    
    

    private void OnOriginCreate(Object obj)
    {
        _op = ((GameObject)obj).transform;
        _trackingActive = true;
    }
    private void OnOriginDestroy(Object obj)
    {
        _op = null;
        _trackingActive = false;
    }
    #endregion
}