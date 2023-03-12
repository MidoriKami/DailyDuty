using System;
using System.ComponentModel;
using System.Resources;
using DailyDuty.System.Localization;

namespace DailyDuty.Models.Attributes;

public class Label : DescriptionAttribute
{
    private readonly string resourceKey;
    private readonly ResourceManager resource;
    
    public Label(string resourceKey)
    {
        resource = Strings.ResourceManager;
        this.resourceKey = resourceKey;
    }

    public override string Description
    {
        get
        {
            var displayName = resource.GetString(resourceKey);

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