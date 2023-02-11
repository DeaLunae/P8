using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.Splines;

public class RuntimeKnotGeneratorRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _knotPrefab;
    private SplineContainer splineContainer;
    private List<GameObject> _knotGameObjects = new(50);
    

    private bool splineDirtyThisFrame;
    
    private GameObject KnotBeingMoved;
    [SerializeField,ReadOnly] private bool isKnotBeingMoved;

    // Start is called before the first frame update
    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        GenerateInitialKnot();
        splineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);
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
                splineContainer.Spline.RemoveAt(i);
                _knotGameObjects.RemoveAt(i);
                i--;
                changed = true;
                continue;
            }
            /*if (!DoesTransformMatchKnot(_knotGameObjects[i].transform.position,_knotGameObjects[i].transform.rotation, _splineContainer.Spline[i]))
            {
                var knot = new BezierKnot(
                    _knotGameObjects[i].transform.localPosition,
                    float3.zero,
                    float3.zero,
                    Quaternion.Euler(Vector3.forward));
                _splineContainer.Spline.RemoveAt(i);
                _splineContainer.Spline.Insert(i,knot);
                changed = true;
            }*/
        }

        splineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    private bool DoesTransformMatchKnot(Vector3 p, Quaternion q, BezierKnot k)
    {
        float TOLERANCE = 0.01f;
        return Math.Abs(p.x - k.Position.x) < TOLERANCE &&
               Math.Abs(p.y - k.Position.y) < TOLERANCE &&
               Math.Abs(p.z - k.Position.z) < TOLERANCE &&
               q == k.Rotation;
    }

    public void AddPointToEnd(Vector3 position)
    {
        var bezierKnot = new BezierKnot(transform.InverseTransformPoint(position));
        splineContainer.Spline.Add(bezierKnot,TangentMode.AutoSmooth);
        var knot = Instantiate(_knotPrefab,position,Quaternion.identity,transform);
        _knotGameObjects.Add(knot);
    }
    private void GenerateInitialKnot()
    {
        splineContainer.Spline.Add(new BezierKnot(float3.zero),TangentMode.AutoSmooth);
        var initialKnot = Instantiate(_knotPrefab,transform.TransformPoint(0,0,0),Quaternion.Euler(Vector3.forward),transform);
        _knotGameObjects.Add(initialKnot);
    }
}
