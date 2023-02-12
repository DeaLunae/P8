using Devkit.Modularis.Variables;
using UnityEditor;


namespace DevKit.Modularis.Editor.Variables
{
    [CustomEditor(typeof(BaseVariable),true)]
    public class BaseValueDrawer : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                ((BaseVariable)target).InvokeCallback();
            }
        }
    }
}