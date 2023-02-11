using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class CheckIfShouldSlotOntoSpline : MonoBehaviour
{
    private Collider _collider;
    private SplineAnimate _splineAnimate;
    private RuntimeKnotGenerator currentKnotGenerator;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private float movementSpeed = 1f;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    public void OnTranslateEnd()
    {
        var knot = CheckIfKnotNearby();
        
        if (knot == null)
        {
            //We aren't touching anything
            if (currentKnotGenerator == null) return;
            currentKnotGenerator = null;            
            //but we were at some point, so discard that
            _splineAnimate.enabled = false;
            _splineAnimate.Container = null;
            return;
        }
        //we are touching something
        if (currentKnotGenerator != null && knot == currentKnotGenerator)
        {
            //but its the same as we were touching before
            return;
        }
        //we are touching something but its not the same as before
        if (currentKnotGenerator != null)
        {
            _splineAnimate.enabled = false;
            _splineAnimate.Container = null;
        }
        _splineAnimate.Container = knot.GetComponent<SplineContainer>();
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.Duration = _splineAnimate.Container.CalculateLength() * movementSpeed;
        _splineAnimate.enabled = true;
        currentKnotGenerator = knot;
    }

    public RuntimeKnotGenerator CheckIfKnotNearby()
    {
        var average = _collider.bounds.size.x + _collider.bounds.size.y + _collider.bounds.size.z;
        average = (average / 3) * 1.2f;
        var colliders = Physics.OverlapSphere(gameObject.transform.position, 10f, LayerMask,QueryTriggerInteraction.Collide);
        if (colliders.Length == 0)
        {
            return null;
        }
        colliders = colliders.Where(e => e.gameObject.CompareTag("Spline")).ToArray();
        if (colliders.Length == 0)
        {
            return null;
        }
        var result = colliders.OrderBy(e => Vector3.Distance(gameObject.transform.position, e.transform.position)).FirstOrDefault();
        return result == null ? null : result.GetComponent<RuntimeKnotGenerator>();
    }
}
