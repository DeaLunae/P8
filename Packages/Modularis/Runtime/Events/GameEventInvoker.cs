using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Devkit.Modularis.Events
{
    public class GameEventInvoker : MonoBehaviour
    {
        [Tooltip("Event to invoke.")]
        [SerializeField] private GameEvent gameEvent;
        public bool EventAssigned => gameEvent != null;
        public string EventName => gameEvent.name;
        public void OnEventRaised()
        {
            if (!Application.isPlaying || gameEvent == null) return;
            gameEvent.Invoke();
        }
    }
}