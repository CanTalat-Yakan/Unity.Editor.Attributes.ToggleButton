using System;
using UnityEngine;

namespace UnityEssentials
{
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