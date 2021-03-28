using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class ConditionalShowInInspectorAttribute : PropertyAttribute
    {
        public string FieldName;
        public bool TargetState;
        
        public ConditionalShowInInspectorAttribute(string targetField, bool targetValue)
        {
            FieldName = targetField;
            TargetState = targetValue;
        }
    }
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConditionalShowInInspectorAttribute))]
    public class ConditionalShowInInspectorDrawer : PropertyDrawer
    {
        private bool _shown;
        private ConditionalShowInInspectorAttribute _attribute;
        private Object _targetObject;
        private string _propertyPath = null;
        private string _conditionTargetPropertyPath = null;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_attribute == null)
            {
                _attribute = (ConditionalShowInInspectorAttribute) attribute;
                _targetObject = property.serializedObject.targetObject;
            }
            
            // if (_propertyPath == null)
            {
                int index;
                _propertyPath = property.propertyPath.Substring(0, (index = property.propertyPath.LastIndexOf("."))<0?property.propertyPath.Length:index);
                
                if (property.propertyPath[property.propertyPath.Length - 1] == ']')
                {
                    _propertyPath = _propertyPath.Substring(0, _propertyPath.LastIndexOf("."));
                    _propertyPath = _propertyPath.Substring(0, _propertyPath.LastIndexOf("."));
                }
            
                _conditionTargetPropertyPath = $"{_propertyPath}.{_attribute.FieldName}";
            }
            
            SerializedProperty prop =
                (new SerializedObject(property.serializedObject.targetObject)).FindProperty(
                    _conditionTargetPropertyPath);

            if (prop == null)
            {
                Type targetType = _targetObject.GetType();
                FieldInfo boolField = null;
                while (boolField == null && targetType != null)
                {
                    boolField = targetType.GetField(_attribute.FieldName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    targetType = targetType.BaseType;
                }
                
                if (boolField != null)
                    _shown = ((bool)boolField.GetValue(_targetObject)).Equals(_attribute.TargetState);
                else
                {
                    Debug.LogError($"Field {_attribute.FieldName} could not be found on {_targetObject.name}!");
                    _shown = true;
                }
            }
            else
            {
                _shown = prop.boolValue.Equals(_attribute.TargetState);
            }
     
            if (_shown)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                // EditorGUI.PropertyField(position, property, label);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _shown ? base.GetPropertyHeight(property, label) : 0;
        }
    }
    #endif
}
