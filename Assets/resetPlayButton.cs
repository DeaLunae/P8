using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class resetPlayButton : MonoBehaviour
{
    [SerializeField] private Interactable playButton;
    
    public void OnResetPlayButton()
    {
        if (playButton.IsToggled)
        {
            playButton.TriggerOnClick(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
