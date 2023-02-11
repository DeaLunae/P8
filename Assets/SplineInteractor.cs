using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Unity.VisualScripting;
using UnityEngine;

public class SplineInteractor : GenericInteractor
{
    // Update is called once per frame
    void Update()
    {
        if (_interactorActive)
        {
        }
    }

    private protected override void OnInteractorDisable()
    {
        throw new NotImplementedException();
    }

    private protected override void OnInteractorEnable()
    {
        throw new NotImplementedException();
    }
}
