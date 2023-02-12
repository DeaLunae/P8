using Devkit.Modularis.Events;
using UnityEditor;
using UnityEngine;

namespace DevKit.Modularis.Editor.Events
{
    [CustomEditor(typeof(GameEventInvokerAdvanced))]
    public class GameEventInvokerAdvancedEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var eventP = serializedObject.FindProperty("gameEvent");
            var configP = serializedObject.FindProperty("configuration");
            
            eventP.objectReferenceValue = EditorGUILayout.ObjectField(
                "Game Event",
                eventP.objectReferenceValue, 
                typeof(GameEvent), 
                allowSceneObjects:false);
            
            configP.intValue = (int) (GameEventInvokerAdvanced.InvokerConfiguration)
                EditorGUILayout.EnumFlagsField(
                    "InvokerConfiguration",
                    (GameEventInvokerAdvanced.InvokerConfiguration) configP.intValue);


            if (Application.isPlaying)
            {
                var advancedInvoker = (GameEventInvokerAdvanced)target;
                if (advancedInvoker != null)
                {
                    GUI.enabled = advancedInvoker.EventAssigned;
                    var guiContent = new GUIContent("Invoke GameEvent", "Click this button to invoke the assigned GameEvent");
                    if (!advancedInvoker.EventAssigned)
                    {
                        guiContent.text = "No GameEvent Assigned";
                        guiContent.tooltip = "Cannot invoke without a GameEvent assigned!\n" +
                                             "Click this button to invoke the assigned GameEvent";
                    }
                    if (GUILayout.Button(guiContent))
                    {
                        advancedInvoker.OnEventRaised();
                    }
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
