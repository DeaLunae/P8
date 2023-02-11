using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RuntimeKnotGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _knotPrefab;
    [SerializeField] private Transform childList;
    [SerializeField,ReadOnly] private bool isKnotBeingMoved;
    [SerializeField] private float _curvyness;
    private List<GameObject> _knotGameObjects = new(50);
    
    private SplineExtrude _splineExtrude;

    private bool splineDirtyThisFrame;
    
    private GameObject KnotBeingMoved;

    private Vector3 initialPosition;
    private GameObject knotSpawner;

    [SerializeField] private float _minimumDistance;

    // Start is called before the first frame update
    void Start()
    {
        _splineExtrude = GetComponent<SplineExtrude>();
        GenerateInitialKnot();
        _splineExtrude.Spline.SetTangentMode(TangentMode.AutoSmooth);
        knotSpawner = transform.Find("KnotSpawner").gameObject;
    }

    private void Update()
    {
        UpdateSpline();
    }

    private void UpdateSpline()
    {
        var changed = false;
        for (var i = 0; i < _knotGameObjects.Count; i++)
        {
            if (_knotGameObjects[i] == null)
            {
                _splineExtrude.Spline.RemoveAt(i);
                _knotGameObjects.RemoveAt(i);
                i--;
                changed = true;
                _splineExtrude.Rebuild();
            }
        }
        if (changed)
        {
            _splineExtrude.Spline.SetTangentMode(TangentMode.AutoSmooth);
        }
        
    }

    public void OnKnotMoved()
    {
        for (var i = 0; i < _knotGameObjects.Count; i++)
        {
            if (_knotGameObjects[i] == null)
            {
                _splineExtrude.Spline.RemoveAt(i);
                _knotGameObjects.RemoveAt(i);
                i--;
                _splineExtrude.Rebuild();
            }
            else
            {
                var knot = new BezierKnot(transform.InverseTransformPoint(_knotGameObjects[i].transform.position));
                _splineExtrude.Spline.SetKnot(i, knot);
            }
        }
        UpdateTension();
        _splineExtrude.Rebuild();
    }


    public void OnSpawnerTranslateBegun(Vector3 position)
    {
        initialPosition = position;
    }
    public void AddPointToEnd(Vector3 position)
    {
        if (_splineExtrude.Spline.Count > 2 && !_splineExtrude.Spline.Closed && CheckIfNewPointShouldCloseTheLoop() )
        {
            _splineExtrude.Spline.Closed = true;
            UpdateTension();
            _knotGameObjects.Last().layer = LayerMask.NameToLayer("Interactable");
            _splineExtrude.Rebuild();
            Destroy(knotSpawner);
            return;
        }
        
        var bezierKnot = new BezierKnot(transform.InverseTransformPoint(position));
        _splineExtrude.Spline.Add(bezierKnot,TangentMode.AutoSmooth);
        UpdateTension();
        var knot = Instantiate(_knotPrefab,position,Quaternion.identity,childList);

        if (_knotGameObjects.Count > 1)
        {
            _knotGameObjects.Last().layer = LayerMask.NameToLayer("Interactable");
        }
        _knotGameObjects.Add(knot);
        _knotGameObjects.Last().layer = LayerMask.NameToLayer("Default");
        _splineExtrude.Rebuild();
    }

    private bool CheckIfNewPointShouldCloseTheLoop()
    {
        return knotSpawner.transform.GetComponent<Collider>().bounds
            .Intersects(transform.parent.gameObject.GetComponent<Collider>().bounds);
    }
    private void GenerateInitialKnot()
    {
        var knot = new BezierKnot(float3.zero);
        _splineExtrude.Spline.Add(knot,TangentMode.AutoSmooth);
        UpdateTension();
        var initialKnot = Instantiate(_knotPrefab,transform.TransformPoint(0,0,0),Quaternion.Euler(Vector3.forward),childList);
        initialKnot.GetComponent<GenericInteractable>().SetDestroyable(false);
        initialKnot.layer = LayerMask.NameToLayer("SplineRoot");
        _knotGameObjects.Add(initialKnot);
        _splineExtrude.Rebuild();
    }

    private void UpdateTension()
    {
        for (int i = 0; i < _splineExtrude.Spline.Count; i++)
        {
            _splineExtrude.Spline.SetAutoSmoothTension(i, _curvyness);
        }
    }

    public void OnKnotDestroyed(GameObject o)
    {
        var index = _knotGameObjects.IndexOf(o);
        if (index != -1)
        {
            _knotGameObjects.RemoveAt(index);
            _splineExtrude.Spline.RemoveAt(index);
            _splineExtrude.Rebuild();
        }
    }
}
