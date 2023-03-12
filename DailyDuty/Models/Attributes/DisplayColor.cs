using System;
using System.ComponentModel;
using System.Drawing;

namespace DailyDuty.Models.Attributes;

public class DisplayColor : DescriptionAttribute
{
    public KnownColor Color { get; }

    public DisplayColor(KnownColor color)
    {
        Color = color;
    }
}

public static partial class EnumExtensions
{
    public static Color GetColor(this Enum enumValue) 
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var attributes = fieldInfo?.GetCustomAttributes(typeof(DisplayColor), false) as DisplayColor[] ?? null;

        if (attributes is { Length: > 0 })
        {
            return Color.FromKnownColor(attributes[0].Color);
        }
        else
        {
            return Color.FromKnownColor(KnownColor.White);
        }
    }
}