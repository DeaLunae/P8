using System;
using UnityEngine;

namespace Devkit.Modularis.Variables
{
    [Serializable]
    public abstract class BaseVariable : SerializableScriptableObject
    {
        public abstract void InvokeCallback();
    }
    
    [Serializable]
    public abstract class BaseVariable<T> : BaseVariable, ISerializationCallbackReceiver
    {
        [Multiline] public string description = "";
        [SerializeField] private T value;
        [NonSerialized] private T _runtimeValue;
        protected Action<T> _valueChangeCallback;

        public void OnAfterDeserialize() => _runtimeValue = value;

        public void OnBeforeSerialize() { }

        public T Value
        {
            get => _runtimeValue;
            set
            {
                _runtimeValue = value;
                _valueChangeCallback?.Invoke(value);
            }
        }
        
        public override void InvokeCallback()
        {
            _valueChangeCallback?.Invoke(value);
        }
        
        public void RegisterCallback(Action<T> action)
        {
            _valueChangeCallback += action;
        }
        
        public void UnregisterCallback(Action<T> action)
        {
            _valueChangeCallback -= action;
        }
    }
}