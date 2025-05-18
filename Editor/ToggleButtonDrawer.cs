#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
    public class ToggleButtonDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 32;
        private const float ButtonHeight = 20;

        private bool _hasHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Use ToggleButton with bool only.");
                return;
            }

            var attribute = (ToggleButtonAttribute)base.attribute;
            string group = attribute.GroupName ?? property.name;

            if (attribute.GroupName == null)
            {
                DrawSingleButton(position, property, label.text, attribute.IconName);
                _hasHeight = true;
            }
            else
            {
                var groupProps = FindGroupedProperties(property.serializedObject, group);
                if (groupProps.Count == 0) return;

                _hasHeight = property.propertyPath == groupProps[0].propertyPath;
                if (!_hasHeight) return;

                DrawGroupedButtons(position, group, groupProps);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            _hasHeight
                ? EditorGUIUtility.singleLineHeight + Mathf.Max(0, ButtonHeight - EditorGUIUtility.singleLineHeight + 2)
                : 0;

        private void DrawSingleButton(Rect position, SerializedProperty property, string label, string iconName)
        {
            Rect labelRect = new(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label, GetLabelStyle());

            GUIContent icon = EditorGUIUtility.IconContent(iconName);
            icon.tooltip = GetTooltip(property);

            float x = position.x + EditorGUIUtility.labelWidth + 1;
            Rect buttonRect = new(x, position.y, ButtonWidth + 2, ButtonHeight + 2);
            bool newValue = GUI.Toggle(buttonRect, property.boolValue, icon, "Button");
            if (newValue != property.boolValue)
                property.boolValue = newValue;
        }

        private void DrawGroupedButtons(Rect position, string label, List<SerializedProperty> properties)
        {
            Rect labelRect = new(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label, GetLabelStyle());

            float x = position.x + EditorGUIUtility.labelWidth + 1;
            foreach (var property in properties)
            {
                var attribute = GetAttribute(property);
                if (attribute == null) continue;

                Rect buttonRect = new(x, position.y, ButtonWidth + 2, ButtonHeight + 2);
                GUIContent icon = EditorGUIUtility.IconContent(attribute.IconName);
                icon.tooltip = GetTooltip(property);

                bool newValue = GUI.Toggle(buttonRect, property.boolValue, icon, "Button");
                if (newValue != property.boolValue)
                    property.boolValue = newValue;

                x += ButtonWidth + 4;
            }
        }

        private List<SerializedProperty> FindGroupedProperties(SerializedObject obj, string group)
        {
            List<SerializedProperty> result = new();
            SerializedProperty iterator = obj.GetIterator();
            iterator.NextVisible(true);

            while (iterator.NextVisible(false))
            {
                var attribute = GetAttribute(iterator);
                if (attribute != null && (attribute.GroupName ?? iterator.name) == group)
                    result.Add(iterator.Copy());
            }

            return result;
        }

        private ToggleButtonAttribute GetAttribute(SerializedProperty property)
        {
            var field = fieldInfo.DeclaringType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetCustomAttribute<ToggleButtonAttribute>();
        }

        private string GetTooltip(SerializedProperty property)
        {
            var field = fieldInfo.DeclaringType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var tooltip = field?.GetCustomAttribute<TooltipAttribute>();
            return tooltip?.tooltip ?? ObjectNames.NicifyVariableName(property.name);
        }

        GUIStyle _labelStyle;
        private GUIStyle GetLabelStyle() =>
            _labelStyle ??= new GUIStyle(EditorStyles.label) { fontSize = 12, normal = { textColor = new Color(0.85f, 0.85f, 0.85f) },  };
    }
}
#endif