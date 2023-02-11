using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class RotateAroundObject : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _objectEventReference;
    [SerializeField] private float distanceToRotateFrom;
    [SerializeField] private float RotationSpeed;
    private GameObject _objectToRotateAround;
    private bool objectPresent;

    // Start is called before the first frame update
    void Start()
    {
        _objectEventReference.RegisterListener(OnObjectCreated,OnObjectDestroyed);
    }

    private void OnObjectCreated(Object o)
    {
        _objectToRotateAround = (GameObject)o;
        transform.LookAt(_objectToRotateAround.transform);
        objectPresent = true;
    }
    
    private void OnObjectDestroyed(Object o)
    {
        _objectToRotateAround = null;
        objectPresent = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (objectPresent)
        {
            transform.RotateAround(_objectToRotateAround.transform.position,Vector3.up, RotationSpeed * Time.deltaTime);
            transform.LookAt(_objectToRotateAround.transform);
        }
    }
}
