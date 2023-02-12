using System;
using UnityEngine;

namespace Devkit.Modularis.Variables
{
    public abstract class EnumVariable<TEnumType> : BaseVariable<TEnumType> where TEnumType : Enum { }
    
}