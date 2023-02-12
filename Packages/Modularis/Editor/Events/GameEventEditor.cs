using Devkit.Modularis.Events;
using UnityEditor;
using UnityEngine;

namespace DevKit.Modularis.Editor.Events
{
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Invoke GameEvent"))
            {
                var e = target as GameEvent;
                if (e == null) return;
                e.Invoke();
            }
        }
    }
}
