using UnityEngine;
using UnityEngine.Events;

namespace Devkit.Modularis.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("GameEvent to register with.")]
        [SerializeField] private GameEvent gameEvent;
        
        [Tooltip("Response to invoke when GameEvent is raised.")]
        [SerializeField] private UnityEvent response;

        
        private void OnEnable()
        {
            if (gameEvent == null) return;
            gameEvent.RegisterListener(OnGameEventInvoked);
        }

        private void OnDisable()
        {
            if (gameEvent == null) return;
            gameEvent.UnregisterListener(OnGameEventInvoked);
        }
        
        public void OnGameEventInvoked()
        {
            response.Invoke();
        }
    }
}