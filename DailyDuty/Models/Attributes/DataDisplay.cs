using System;

namespace DailyDuty.Models.Attributes;

public class DataDisplay : Attribute
{
    public string Label { get; }

    public DataDisplay(string label)
    {
        Label = label;
    }
}