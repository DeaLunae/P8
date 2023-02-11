using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInPlaceOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 startpos;
    void Start()
    {
        Invoke(nameof(turnoffatinitialize),0.1f);
    }

    public void turnoffatinitialize()
    {
        gameObject.SetActive(false);
    }
}
