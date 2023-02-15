using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Variables;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = System.Object;

namespace Devkit.Modularis.Channels
{
    [Serializable]
    public abstract class ObjectChannelBase<T> : BaseVariable where T : UnityEngine.Object
    {
        [Multiline] public string description = "";
        private Action _valueChangeCallback = delegate {  };
        public bool isSet = false; // Any script can check if the Object is null before using it, by just checking this bool
        [SerializeField] protected T value;


        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                isSet = value != null;
                _valueChangeCallback?.Invoke();
            }
        }
        
        private void OnDisable()
        {
            Unset();
        }
        public static implicit operator T(ObjectChannelBase<T> channel)
        {
            return channel.Value;
        }
        public virtual void Unset()
        {
            value = null;
            isSet = false;
        }
        public override void InvokeCallback()
        {
            _valueChangeCallback?.Invoke();
        }
        

        public void RegisterCallback(Action action)
        {
            _valueChangeCallback += action;
        }
        
        public void UnregisterCallback(Action action)
        {
            _valueChangeCallback -= action;
        }
    }
}