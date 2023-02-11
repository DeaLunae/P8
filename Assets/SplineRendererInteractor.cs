using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;
using Object = UnityEngine.Object;

public class SplineRendererInteractor : MonoBehaviour
{
    [SerializeField] private GestureIdReference currentEventReference;
    [SerializeField] private ObjectReference splineSpawnTransform;
    private GameObject currentSplineSpawnAnchor;
    [SerializeField] private float distanceBetweenKnots;
    [SerializeField] private GameObject splinePrefab;
    [SerializeField] private Gesture.GestureID gestureThatEnabled;
    private bool GestureActive,HandTrackingActive,SplineSpawned;
    private Vector3 lastKnotPosition;
    private RuntimeKnotGeneratorRenderer _runtimeKnotGenerator;
    // Start is called before the first frame update
    void OnEnable()
    {
        splineSpawnTransform.RegisterCallback(OnSplineTransformChanged);
        currentEventReference.RegisterCallback(OnGestureChanged);
    }

    private void OnGestureChanged(Gesture.GestureID gestureID)
    {
        GestureActive = currentEventReference.Value == gestureThatEnabled;
        SplineSpawned = false;
    }

    private void OnDestroy()
    {
        splineSpawnTransform.UnregisterCallback(OnSplineTransformChanged);
    }

    private void OnSplineTransformChanged(Object o)
    {
        currentSplineSpawnAnchor = (GameObject)splineSpawnTransform.Value;
        HandTrackingActive = currentSplineSpawnAnchor != null;
    }
    // Update is called once per frame
    void Update()
    {
        if(!HandTrackingActive) { return; }
        if (GestureActive && !SplineSpawned)
        {
            SplineSpawned = true;
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;

            var splineGo = Instantiate(splinePrefab,lastKnotPosition,Quaternion.identity);
            _runtimeKnotGenerator = splineGo.GetComponent<RuntimeKnotGeneratorRenderer>();
        }
        print(Vector3.Distance(lastKnotPosition, currentSplineSpawnAnchor.transform.position));
        if (GestureActive && Vector3.Distance(lastKnotPosition, currentSplineSpawnAnchor.transform.position) > distanceBetweenKnots)
        {
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;
            _runtimeKnotGenerator.AddPointToEnd(lastKnotPosition);
        }
    }
}
