using System;
using Devkit.Modularis.Variables;
using UnityEngine;

namespace Devkit.Modularis.References
{
    [Serializable]
    public abstract class EnumReference<TEnumType> : BaseReference<EnumVariable<TEnumType>,TEnumType> where TEnumType : Enum
    {
    }
}