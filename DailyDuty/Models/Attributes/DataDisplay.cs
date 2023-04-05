using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Models.Attributes;

public class DataDisplay : Attribute
{
    private readonly string resourceKey;
    
    public string Label
    {
        get
        {
            var displayName = Strings.ResourceManager.GetString(resourceKey, Strings.Culture);

            return string.IsNullOrEmpty(displayName) ? $"[[{resourceKey}]]" : displayName;
        }
    }
    
    public DataDisplay(string resourceKey)
    {
        this.resourceKey = resourceKey;
    }
}