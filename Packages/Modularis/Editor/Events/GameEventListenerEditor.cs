using Devkit.Modularis.Events;
using UnityEditor;
using UnityEngine;

namespace DevKit.Modularis.Editor.Events
{
    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var eventP = serializedObject.FindProperty("gameEvent");
            var responseP = serializedObject.FindProperty("response");
            
            eventP.objectReferenceValue = EditorGUILayout.ObjectField("Game Event",eventP.objectReferenceValue, typeof(GameEvent),false);

            
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Fire Response"))
            {
                var gameEventListener = target as GameEventListener;
                if (gameEventListener != null)
                {
                    gameEventListener.OnGameEventInvoked();
                }
            }
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(responseP,true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
