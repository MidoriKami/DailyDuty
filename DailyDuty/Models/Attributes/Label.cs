using System;
using System.ComponentModel;
using System.Reflection;
using DailyDuty.System.Localization;
using Dalamud.Utility;

namespace DailyDuty.Models.Attributes;

public class Label : DescriptionAttribute
{
    private readonly string resourceKey;
    
    public Label(string resourceKey)
    {
        this.resourceKey = resourceKey;
    }

    public override string Description
    {
        get
        {
            var displayName = Strings.ResourceManager.GetString(resourceKey);

            return string.IsNullOrEmpty(displayName) ? $"[[{resourceKey}]]" : displayName;
        }
    }
}

public static partial class EnumExtensions
{
    public static string GetLabel(this Enum enumValue) 
    {
        var labelAttribute = enumValue.GetAttribute<Label>();

        return labelAttribute is not null ? labelAttribute.Description : enumValue.ToString();
    }
}