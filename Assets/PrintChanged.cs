using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintChanged : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        print(transform.hasChanged);
    }
}
