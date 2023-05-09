using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetFlip : MonoBehaviour
{
    private FlipAll _flipAll;
    // Start is called before the first frame update
    void Start()
    {
        _flipAll = FindObjectOfType<FlipAll>();
    }

    public void ResetFlipAll()
    {
        _flipAll.ResetFlips();
    }
}
