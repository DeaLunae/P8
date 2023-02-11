using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

public class Rotate : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _ObjectToRotateAround; 
    // Start is called before the first frame update
    [SerializeField,UnityEngine.Min(1)] private uint degreesPerSecond;
    private Transform _transform;

    private void Start()
    {
        _ObjectToRotateAround.RegisterListener(OnCreated,OnDestroyed);
    }

    private void OnCreated(Object obj)
    {
        _transform = ((GameObject)obj).transform;
    }

    private void OnDestroyed(Object obj)
    {
        _transform = null;
    }

    void Update()
    {
        if (_transform != null)
        {
            transform.position = CalculateMiddle(_transform);
            transform.Rotate(new Vector3(0,1,0),Time.deltaTime*3.6f*degreesPerSecond);
        }
    }

    public Vector3 CalculateMiddle(Transform t)
    {
        var pos = Vector3.zero;
        var childpos = t.GetComponentsInChildren<Transform>().Select(e => e.position).ToList();
        foreach (var child in childpos)
        {
            pos += child;
        }
        pos /= childpos.Count;
        return pos;
    }
}
