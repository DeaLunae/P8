using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Variables;
using UnityEngine;

[CreateAssetMenu(menuName = "Core/Variables/AnimationState Variable",fileName = "AnimationState Variable",order = 100)]
public class AnimationStateVariable : BaseVariable<AnimationState>
{
   
}
[Serializable]
public enum AnimationState
{
    Stopped = 0,
    Playing = 1,
    Paused = 2
}
