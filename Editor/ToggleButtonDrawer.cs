//https://discussions.unity.com/t/property-drawer-for-enum-flags-masks-download/691923/14
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    /// <summary>
    /// Provides a custom property drawer for fields decorated with the <see cref="ToggleButtonAttribute"/>.
    /// </summary>
    /// <remarks>This drawer renders a toggle button in the Unity Inspector for boolean fields. If the <see
    /// cref="ToggleButtonAttribute"/> specifies a group name, it renders grouped toggle buttons for all fields in the
    /// same group. The drawer supports optional icons and tooltips for the buttons.</remarks>
    [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
    public class ToggleButtonDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 32;
        private const float ButtonHeight = 20;

        private bool _requiresHeightAdjustment;

        /// <summary>
        /// Renders a custom toggle button in the Unity Inspector for a serialized boolean property.
        /// </summary>
        /// <remarks>This method supports both standalone toggle buttons and grouped toggle buttons. If
        /// the <c>ToggleButtonAttribute</c> specifies a group name, all properties in the same group will be rendered
        /// as a set of grouped buttons. Otherwise, a single toggle button is rendered for the property. <para> If the
        /// property is not of type <see cref="SerializedPropertyType.Boolean"/>, an error message is displayed in the
        /// Inspector. </para></remarks>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.HelpBox(position, "ToggleButton attribute only supports bool fields.", MessageType.Error);
                return;
            }

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

        /// <summary>
        /// Calculates the height required to render the property in the editor, including any additional space for
        /// custom UI elements.
        /// </summary>
        /// <param name="property">The serialized property for which the height is being calculated.</param>
        /// <param name="label">The label associated with the property, typically displayed in the editor.</param>
        /// <returns>The height, in pixels, required to render the property. Returns <see langword="0"/> if no height adjustment
        /// is needed.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            _requiresHeightAdjustment ? EditorGUIUtility.singleLineHeight + Mathf.Max(0, ButtonHeight - EditorGUIUtility.singleLineHeight + 2) : 0;

        /// <summary>
        /// Renders a single button with an optional label and icon in the Unity Inspector.
        /// </summary>
        /// <remarks>The button toggles the value of the specified boolean property. If a label is
        /// provided, it is displayed to the left of the button. The icon, if specified, is displayed on the button, and
        /// its tooltip is derived from the property's tooltip.</remarks>
        /// <param name="position">The screen position and size of the button, including the label if specified.</param>
        /// <param name="property">The serialized boolean property that the button toggles.</param>
        /// <param name="label">The optional label to display next to the button. Can be <see langword="null"/> or empty to omit the label.</param>
        /// <param name="iconName">The name of the icon to display on the button. Can be <see langword="null"/> or empty to omit the icon.</param>
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

            var buttonPosition = new Rect(position.x, position.y, ButtonWidth + 2, ButtonHeight + 2);
            var newValue = GUI.Toggle(buttonPosition, property.boolValue, icon, "Button");
            if (InspectorFocusedHelper.ProcessKeyboardClick(buttonPosition))
                newValue = !newValue;

            property.boolValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws a group of toggle buttons within the specified position, each corresponding to a serialized property.
        /// </summary>
        /// <remarks>Each button is rendered with an icon specified by the <see
        /// cref="ToggleButtonAttribute"/> applied to the property. Clicking a button toggles the associated property's
        /// boolean value. Keyboard interactions are also supported for toggling.</remarks>
        /// <param name="position">The rectangular area on the screen where the buttons will be drawn.</param>
        /// <param name="label">The label displayed to the left of the group of buttons.</param>
        /// <param name="properties">A list of <see cref="SerializedProperty"/> objects, each representing a property to be displayed as a toggle
        /// button. Only properties with a <see cref="ToggleButtonAttribute"/> will be rendered.</param>
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
                if (InspectorFocusedHelper.ProcessKeyboardClick(buttonPosition))
                    newValue = !newValue;
                property.boolValue = newValue;
                property.serializedObject.ApplyModifiedProperties();

                xOffset += ButtonWidth + 4;
            }
        }
    }
}
#endif