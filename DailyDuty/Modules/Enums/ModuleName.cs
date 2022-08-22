using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Modules.Enums;

public enum ModuleName
{
    BeastTribe,
    CustomDelivery
}

public static class ModuleNameExtensions
{
    public static string GetLocalizedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.BeastTribe => Strings.Module.BeastTribe.Label,
            ModuleName.CustomDelivery => Strings.Module.CustomDelivery.Label,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}