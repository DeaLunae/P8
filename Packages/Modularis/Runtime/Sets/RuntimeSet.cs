using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devkit.Modularis.Sets
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public readonly List<T> Items = new List<T>();
        public Action ValueChangeCallback;

        public void RegisterCallback(Action action)
        {
            ValueChangeCallback += action;
        }
        
        public void UnregisterCallback(Action action)
        {
            ValueChangeCallback -= action;
        }
        
        public void Add(T thing)
        {
            if (Items.Contains(thing)) return;
            Items.Add(thing);
            ValueChangeCallback.Invoke();
        }

        public void Remove(T thing)
        {
            if (!Items.Contains(thing)) return;
            Items.Remove(thing);
            ValueChangeCallback.Invoke();
        }
    }
}