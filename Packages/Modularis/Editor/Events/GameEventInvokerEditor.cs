using Devkit.Modularis.Events;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DevKit.Modularis.Editor.Events
{
    [CustomEditor(typeof(GameEventInvoker))]
    public class GameEventInvokerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var eventP = serializedObject.FindProperty("gameEvent");
            eventP.objectReferenceValue = EditorGUILayout.ObjectField(
                "Game Event",
                eventP.objectReferenceValue, 
                typeof(GameEvent), 
                allowSceneObjects:false);
            var invoker = (GameEventInvoker)target;
            if (invoker != null)
            {
                GUI.enabled = invoker.EventAssigned && Application.isPlaying;
                
                var guiContent = new GUIContent("Invoke GameEvent", "Click this button to invoke the assigned GameEvent");
                if (!invoker.EventAssigned)
                {
                    guiContent.text = "No GameEvent Assigned";
                    guiContent.tooltip = "Cannot invoke without a GameEvent assigned!\n" +
                                         "Click this button to invoke the assigned GameEvent";
                }
                if (GUILayout.Button(guiContent))
                {
                    invoker.OnEventRaised();
                }
            }
            
            GUI.enabled = true;

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
