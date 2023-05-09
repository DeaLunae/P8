using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAll : MonoBehaviour
{
    private FlipAxis[] _flips;

    private bool flipped;

    // Start is called before the first frame update
    void Start()
    {
        _flips = FindObjectsOfType<FlipAxis>();
    }
    
    public void FlipAllAxis()
    {
        foreach (var flip in _flips)
        {
            flip.Flip();
        }
        flipped = !flipped;
    }

    public void ResetFlips()
    {
        if (!flipped) return;
        FlipAllAxis();
    }
}
