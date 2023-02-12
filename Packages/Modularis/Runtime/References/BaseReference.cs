using System;
using System.Reflection;
using Devkit.Modularis.Helpers;
using Devkit.Modularis.Variables;
using UnityEngine;

namespace Devkit.Modularis.References
{
    [Serializable]
    public abstract class BaseReference : object
    {

        public abstract void ResetConstant();
        public abstract object GetConstant();
        public abstract void InvokeConstantValueCallback();
    }
    
    [Serializable]
    public abstract class BaseReference<TVariableType, TBaseType> : BaseReference where TVariableType : BaseVariable<TBaseType>
    {
        public Action<TBaseType> ConstantValueChanged;
        public bool useConstant;
        [SerializeField,HideInInspector] private bool foldout;
        public TBaseType constantValue;

        [SerializeField] private TVariableType variable;

        protected BaseReference() { }

        public override void InvokeConstantValueCallback()
        {
            ConstantValueChanged?.Invoke(constantValue);
        }
        public override void ResetConstant()
        {
            constantValue = Activator.CreateInstance<TBaseType>();
        }
        
        public override object GetConstant()
        {
            return constantValue;
        }

        protected BaseReference(TBaseType value)
        {
            useConstant = true;
            ConstantValue = value;
            Value = value;
        }
        
        public static implicit operator TBaseType(BaseReference<TVariableType,TBaseType> reference)
        {
            return reference.Value;
        }

        public TBaseType Value
        {
            get => variable.Value;

            //get => useConstant ? ConstantValue : Variable.Value;
            set => variable.Value = value;
        }

        public TBaseType ConstantValue
        {
            get => constantValue;
            set
            {
                constantValue = value;
                ConstantValueChanged?.Invoke(value);
            }
        }
        public TVariableType Variable
        {
            get => variable;
            set => variable = value;
        }


        public void RegisterCallback(Action<TBaseType> action)
        {
            ConstantValueChanged += action;
            if (Variable == null) return;
            variable.RegisterCallback(action);
        }
        public void UnregisterCallback(Action<TBaseType> action)
        {
            ConstantValueChanged -= action;
            if (Variable == null) return;
            variable.UnregisterCallback(action);
        }
    }
}
