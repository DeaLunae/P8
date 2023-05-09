using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlipTextIfFlipped : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    void Start()
    {
        if (gestureRecorderEvents == null) return;
        var tmp = GetComponent<TMP_Text>();
        if (tmp.text == "Højre")
        {
            tmp.text = "Venstre";
            tmp.ForceMeshUpdate();
        }else if (tmp.text == "Venstre")
        {
            tmp.text = "Højre";
            tmp.ForceMeshUpdate();
        }
    }

}
