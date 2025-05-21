#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
    public class ToggleButtonDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 32;
        private const float ButtonHeight = 20;

        private bool _requiresHeightAdjustment;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Use ToggleButton with bool only.");
                return;
            }

            InspectorHook.MarkPropertyAsHandled(property.propertyPath);

            var attribute = this.attribute as ToggleButtonAttribute;
            var group = attribute.GroupName ?? property.name;

            if (attribute.GroupName == null)
            {
                DrawSingleButton(position, property, label.text, attribute.IconName);
                _requiresHeightAdjustment = true;
            }
            else
            {
                var groupProperties = new List<SerializedProperty>();
                InspectorHookUtilities.IterateProperties((property) =>
                {
                    if (InspectorHookUtilities.TryGetAttribute<ToggleButtonAttribute>(property, out var attribute))
                        if ((attribute.GroupName ?? property.name) == group)
                            groupProperties.Add(property.Copy());
                });
                if (groupProperties.Count == 0)
                    return;

                _requiresHeightAdjustment = property.propertyPath == groupProperties[0].propertyPath;
                if (!_requiresHeightAdjustment)
                    return;

                DrawGroupedButtons(position, group, groupProperties);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            _requiresHeightAdjustment
                ? EditorGUIUtility.singleLineHeight + Mathf.Max(0, ButtonHeight - EditorGUIUtility.singleLineHeight + 2) : 0;

        private void DrawSingleButton(Rect position, SerializedProperty property, string label, string iconName)
        {
            if (!string.IsNullOrEmpty(label))
            {
                var labelPosition = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelPosition, label);
                position.x += EditorGUIUtility.labelWidth + 1;
            }

            var icon = EditorGUIUtility.IconContent(iconName);
            icon.tooltip = InspectorHookUtilities.GetToolTip(property);

            var xOffset = position.x;
            var buttonPosition= new Rect(xOffset, position.y, ButtonWidth + 2, ButtonHeight + 2);
            var newValue = GUI.Toggle(buttonPosition, property.boolValue, icon, "Button");
            if (newValue != property.boolValue)
                property.boolValue = newValue;
        }

        private void DrawGroupedButtons(Rect position, string label, List<SerializedProperty> properties)
        {
            var labelPosition = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);

            var xOffset = position.x + EditorGUIUtility.labelWidth + 1;
            foreach (var property in properties)
            {
                if (!InspectorHookUtilities.TryGetAttribute<ToggleButtonAttribute>(property, out var attribute))
                    continue;

                var buttonPosition = new Rect(xOffset, position.y, ButtonWidth + 2, ButtonHeight + 2);
                var icon = EditorGUIUtility.IconContent(attribute.IconName);
                icon.tooltip = InspectorHookUtilities.GetToolTip(property);

                var newValue = GUI.Toggle(buttonPosition, property.boolValue, icon, "Button");
                if (newValue != property.boolValue)
                    property.boolValue = newValue;

                xOffset += ButtonWidth + 4;
            }
        }
    }
}
#endif