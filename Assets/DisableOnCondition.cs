using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;

public class DisableOnCondition : MonoBehaviour
{
    [SerializeField] private IntReference _conditionReference;
    [SerializeField] private int requiredCondition;
    void OnEnable()
    {
        if (_conditionReference.Value != requiredCondition)
        {
            gameObject.SetActive(false);
        }
    }
}
