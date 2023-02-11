using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Events;
using UnityEngine;

public class GesturePrinter : MonoBehaviour
{
    [SerializeField] private GestureIdVariable _gestureIdVariable;


    public void GestureStarted()
    {
        print($"Started: {_gestureIdVariable.Value}");
    }

    public void GestureEnded()
    {
        print($"Stopped: {_gestureIdVariable.Value}");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print($"Value: {_gestureIdVariable.Value}");
    }
}
