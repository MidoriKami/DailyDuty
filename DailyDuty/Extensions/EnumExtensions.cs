using System;
using System.ComponentModel;
using Dalamud.Utility;

namespace DailyDuty.Extensions;

public static class EnumExtensions {
    extension(Enum enumValue) {
        public string Description => enumValue.GetDescription();

        private string GetDescription() {
            if (enumValue.GetAttribute<DescriptionAttribute>() is { } attribute) {
                return attribute.Description;
            }
            
            return enumValue.ToString();
        }
    }
    
    public static T Parse<T>(this string stringValue, T defaultValue) where T : Enum {
        foreach (Enum enumValue in Enum.GetValues(typeof(T))) {
            if (enumValue.Description == stringValue) {
                return (T)enumValue;
            }
        }

        return defaultValue;
    }
}
