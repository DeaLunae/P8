using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class GestureLogger : MonoBehaviour
{
    private LoggingManager _loggingManager;

    private MixedRealityPose[] jointPoses;
    private TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.IndexTip;
    // Start is called before the first frame update
    void Start()
    {
        _loggingManager = GetComponent<LoggingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
