using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devkit.Modularis.Events
{
    [CreateAssetMenu(menuName = "Modularis/Game Event",fileName = "Game Event",order = 99)]
    public class GameEvent : SerializableScriptableObject
    {
        #if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
        #endif
        
        private readonly List<Action> _actionEventListeners = new();

        public void Invoke()
        {
            for (var i = _actionEventListeners.Count - 1; i >= 0; i--)
            {
                _actionEventListeners[i].Invoke();
            }
        }
        public void RegisterListener(Action listener)
        {
            if (!_actionEventListeners.Contains(listener))
            {
                _actionEventListeners.Add(listener);
            }
        }
        public void UnregisterListener(Action listener)
        {
            if (_actionEventListeners.Contains(listener))
            {
                _actionEventListeners.Remove(listener);
            }
        }
    }
}