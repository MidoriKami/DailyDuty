using System;

namespace DailyDuty.Models.Attributes;

public class DataDisplay : Attribute
{
    public string Label { get; }
    public string? HelpText { get; }

    public DataDisplay(string label, string? helpText = null)
    {
        Label = label;
        HelpText = helpText;
    }
}