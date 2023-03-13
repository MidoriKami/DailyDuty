using System;

namespace DailyDuty.Models.Attributes;

public class ConfigOption : Attribute
{
    public string Name { get; }
    public string? HelpText { get; }

    public ConfigOption(string name, string? helpText = null)
    {
        Name = name;
        HelpText = helpText;
    }
}