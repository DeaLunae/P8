using System.Linq;
using DevKit.Modularis.Editor.Variables;
using Devkit.Modularis.References;
using UnityEditor;
using UnityEngine;
namespace DevKit.Modularis.Editor.References
{
    [CustomPropertyDrawer(typeof(BaseReference), true)]
    public class BaseReferenceDrawer : PropertyDrawer
    {
        private readonly string[] _popupOptions = { "Use Constant", "Use Variable" };
        private readonly string[] _popup2Options = { "Save Constant to Variable", "Load Constant from Variable", "Reset Constant"};

        private GUIStyle _popupStyleDropdown, _popupStyleButton, _foldout;
        private SerializedProperty _targetProperty;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _targetProperty = property;
            _popupStyleDropdown ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions")) { imagePosition = ImagePosition.ImageOnly };
            _popupStyleButton ??= new GUIStyle(GUI.skin.GetStyle("miniButton"));
            _foldout ??= new GUIStyle(GUI.skin.GetStyle("FoldoutHeader"));

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            
            var useConstantP = property.FindPropertyRelative("useConstant");
            var constantValueP = property.FindPropertyRelative("constantValue");
            var variableP = property.FindPropertyRelative("variable");
            var foldoutP = property.FindPropertyRelative("foldout");
            
            position.height = 16f + EditorGUIUtility.standardVerticalSpacing;
            var configButton = new Rect(position);
            configButton.yMin += _popupStyleDropdown.margin.top;
            configButton.width = _popupStyleDropdown.fixedWidth + _popupStyleDropdown.margin.right;
            configButton.x -= 16f;
            position.xMin = configButton.xMax;
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.Popup(configButton,useConstantP.boolValue ? 0 : 1, _popupOptions, _popupStyleDropdown);
            useConstantP.boolValue = result == 0;
            var baseref = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BaseReference>(fieldInfo, property);
            if (useConstantP.boolValue)
            {
                if (constantValueP.propertyType == SerializedPropertyType.Generic)
                {
                    var genericType = baseref.GetConstant()?.GetType();
                    if (genericType != null)
                    {
                        var genericTypeSpecificName = genericType.FullName.Split(".").Last();
                        foldoutP.boolValue = EditorGUI.BeginFoldoutHeaderGroup(
                            position, 
                            foldoutP.boolValue, 
                            genericTypeSpecificName);
                    
                        if (foldoutP.boolValue)
                        {
                            var newpos = new Rect(position);
                            newpos.y += 16f + EditorGUIUtility.standardVerticalSpacing;
                            var offset = newpos.x;
                            newpos.x = 0f;
                            newpos.width += offset;
                            EditorGUI.indentLevel += 1;
                            var startingDepth = constantValueP.depth;
                            // Move into the first child of constantValue
                            constantValueP.NextVisible(true);
                            do
                            {
                                EditorGUILayout.PropertyField(constantValueP, true);
                                constantValueP.NextVisible(false);
                                // Quit iterating when you are back at the original depth (you've drawn all children)
                            } while (constantValueP.depth > startingDepth);
                        }
                        EditorGUI.EndFoldoutHeaderGroup();
                    }
                }
                else
                {
                    position.xMax -= 22f;
                    EditorGUI.PropertyField(position, constantValueP,new GUIContent(""),includeChildren:false);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, variableP,new GUIContent(""),includeChildren:false);
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                if (useConstantP.boolValue)
                {
                    var myDataClass = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BaseReference>(fieldInfo, property);
                    myDataClass?.InvokeConstantValueCallback();
                }
            }
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}