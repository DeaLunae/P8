using System;
using UnityEngine;

namespace Devkit.Modularis.Events
{
    public class GameEventInvokerAdvanced : MonoBehaviour
    {
        [Tooltip("Event to invoke.")]
        [SerializeField] private GameEvent gameEvent;
        
        [Tooltip("When to evoke this event")]
        [SerializeField,EnumFlags] private InvokerConfiguration configuration;

        public bool EventAssigned => gameEvent != null;

        [Flags]
        public enum InvokerConfiguration
        {
            Default = 0,
            Awake = 1 << 0,
            Start = 1 << 1,
            OnEnable = 1 << 2,
            OnDisable = 1 << 3,
            OnDestroy = 1 << 4,
            Update = 1 << 5,
            FixedUpdate = 1 << 6,
            LateUpdate = 1 << 7,
            OnValidate = 1 << 8,
            OnApplicationFocusGet = 1 << 9,
            OnApplicationFocusLost = 1 << 10,
            OnApplicationPause = 1 << 11,
            OnApplicationUnpause = 1 << 12,
            OnApplicationQuit = 1 << 13,
            OnPreRender = 1 << 14,
            OnPostRender = 1 << 15
        }

        public void OnEventRaised()
        {
            if (!Application.isPlaying || gameEvent == null) return;
            gameEvent.Invoke();
        }
        
        private void Awake()
        {
            if (configuration.HasFlag(InvokerConfiguration.Awake))
            {
                OnEventRaised();
            }
        }
        private void Start()
        {
            if (configuration.HasFlag(InvokerConfiguration.Start))
            {
                OnEventRaised();
            }
        }
        private void OnEnable()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnEnable))
            {
                OnEventRaised();
            }
        }
        private void OnDisable()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnDisable))
            {
                OnEventRaised();
            }
        }
        private void OnDestroy()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnDestroy))
            {
                OnEventRaised();
            }
        }
        private void Update()
        {
            if (configuration.HasFlag(InvokerConfiguration.Update))
            {
                OnEventRaised();
            }
        }
        private void FixedUpdate()
        {
            if (configuration.HasFlag(InvokerConfiguration.FixedUpdate))
            {
                OnEventRaised();
            }
        }
        private void LateUpdate()
        {
            if (configuration.HasFlag(InvokerConfiguration.LateUpdate))
            {
                OnEventRaised();
            }
        }
        private void OnValidate()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnValidate))
            {
                OnEventRaised();
            }
        }
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                if (configuration.HasFlag(InvokerConfiguration.OnApplicationFocusGet))
                {
                    OnEventRaised();
                }
            }
            else
            {
                if (configuration.HasFlag(InvokerConfiguration.OnApplicationFocusLost))
                {
                    OnEventRaised();
                }
            }
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (configuration.HasFlag(InvokerConfiguration.OnApplicationPause))
                {
                    OnEventRaised();
                }
            }
            else
            {
                if (configuration.HasFlag(InvokerConfiguration.OnApplicationUnpause))
                {
                    OnEventRaised();
                }
            }
        }
        private void OnApplicationQuit()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnApplicationQuit))
            {
                OnEventRaised();
            }
        }
        private void OnPreRender()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnPreRender))
            {
                OnEventRaised();
            }
        }
        private void OnPostRender()
        {
            if (configuration.HasFlag(InvokerConfiguration.OnPostRender))
            {
                OnEventRaised();
            }
        }
    }
}