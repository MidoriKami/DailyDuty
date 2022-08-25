using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Modules.Enums;

public enum ModuleName
{
    BeastTribe,
    CustomDelivery,
    DomanEnclave,
    DutyRoulette,
    FashionReport,
    HuntMarksDaily
}

public static class ModuleNameExtensions
{
    public static string GetLocalizedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.BeastTribe => Strings.Module.BeastTribe.Label,
            ModuleName.CustomDelivery => Strings.Module.CustomDelivery.Label,
            ModuleName.DomanEnclave => Strings.Module.DomanEnclave.Label,
            ModuleName.DutyRoulette => Strings.Module.DutyRoulette.Label,
            ModuleName.FashionReport => Strings.Module.FashionReport.Label,
            ModuleName.HuntMarksDaily => Strings.Module.HuntMarks.DailyLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}