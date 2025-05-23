using System;
using UnityEngine;

namespace UnityEssentials
{
    /// <summary>
    /// Specifies that a field should be displayed as a toggle button in the editor, optionally grouped with other
    /// toggle buttons.
    /// </summary>
    /// <remarks>This attribute is typically used to enhance the visual representation of fields in custom
    /// editors by rendering them as toggle buttons. The appearance of the button can be customized using an icon, and
    /// related buttons can be grouped together by specifying a group name.</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ToggleButtonAttribute : PropertyAttribute
    {
        public string IconName { get; }
        public string GroupName { get; }

        public ToggleButtonAttribute(string iconName, string groupName = null)
        {
            IconName = iconName;
            GroupName = groupName;
        }

        public ToggleButtonAttribute(IconNames iconName, string groupName = null)
        {
            IconName = Icon.GetIconReferenceByName(iconName);
            GroupName = groupName;
        }
    }
}