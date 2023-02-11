using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class SetSpriteOnstart : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    void Start()
    {
        GetComponent<ButtonConfigHelper>().SetSpriteIcon(sprite);
    }
}
