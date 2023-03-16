using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Models.Attributes;

public class ConfigOption : Attribute
{
    private readonly string resourceKey;
    private readonly string? helpTextKey;
    
    public string Name
    {
        get
        {
            var displayName = Strings.ResourceManager.GetString(resourceKey);

            return string.IsNullOrEmpty(displayName) ? $"[[{resourceKey}]]" : displayName;
        }
    }

    public string? HelpText 
    {
        get
        {
            if (helpTextKey is null) return null;
            
            var displayName = Strings.ResourceManager.GetString(helpTextKey);

            return string.IsNullOrEmpty(displayName) ? $"[[{helpTextKey}]]" : displayName;
        }
    }

    public int IntMin { get; }
    public int IntMax { get; } = 100;

    public ConfigOption(string resourceKey)
    {
        this.resourceKey = resourceKey;
    }
    
    public ConfigOption(string resourceKey, int intMin, int intMax)
    {
        this.resourceKey = resourceKey;
        IntMin = intMin;
        IntMax = intMax;
    }

    public ConfigOption(string resourceKey, string helpTextKey)
    {
        this.resourceKey = resourceKey;
        this.helpTextKey = helpTextKey;
    }
}