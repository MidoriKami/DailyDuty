using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Models.Attributes;

public class ClickableLink : Attribute
{
    private readonly string descriptionKey;

    public string Description
    {
        get
        {
            var displayName = Strings.ResourceManager.GetString(descriptionKey, Strings.Culture);

            return string.IsNullOrEmpty(displayName) ? $"[[{descriptionKey}]]" : displayName;
        }
    }
    
    public ClickableLink(string descriptionKey)
    {
        this.descriptionKey = descriptionKey;
    }
}