using System;
using System.ComponentModel;
using System.Resources;
using DailyDuty.System.Localization;

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
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var attributes = fieldInfo?.GetCustomAttributes(typeof(Label), false) as Label[] ?? null;

        return attributes is { Length: > 0 } ? attributes[0].Description : enumValue.ToString();
    }
}