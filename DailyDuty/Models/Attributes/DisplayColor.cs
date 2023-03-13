using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

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
    public static Vector4 GetColor(this Enum enumValue) 
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var attributes = fieldInfo?.GetCustomAttributes(typeof(DisplayColor), false) as DisplayColor[] ?? null;

        return attributes is { Length: > 0 } ? attributes[0].Color.AsVector4() : KnownColor.White.AsVector4();
    }

    public static Vector4 AsVector4(this KnownColor enumValue)
    {
        var enumColor = Color.FromKnownColor(enumValue);
        return new Vector4(enumColor.R / 255.0f, enumColor.G / 255.0f, enumColor.B / 255.0f, enumColor.A / 255.0f);
    }
}