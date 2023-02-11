using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Variables;
using UnityEngine;

[CreateAssetMenu(menuName = "Core/Variables/Editing State Variable",fileName = "Editing State Variable",order = 100)]
public class EditingStateVariable : BaseVariable<EditingState>
{
   
}

[Serializable]
public enum EditingState
{
    Object,
    Spline
}
