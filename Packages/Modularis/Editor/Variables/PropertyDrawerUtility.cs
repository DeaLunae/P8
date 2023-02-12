using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace DevKit.Modularis.Editor.Variables
{
    public static class PropertyDrawerUtility
    {
        public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
        {
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (obj == null) { return null; }
 
            T actualObject;
            if (obj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(char.IsDigit).ToArray()));
                actualObject = ((T[])obj)[index];
            }
            else
            {
                actualObject = obj as T;
            }
            return actualObject;
        }
    }
}