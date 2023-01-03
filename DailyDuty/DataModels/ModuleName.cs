using System;
using DailyDuty.Localization;

namespace DailyDuty.DataModels;

public enum ModuleName
{
    BeastTribe,
    CustomDelivery,
    DomanEnclave,
    DutyRoulette,
    FashionReport,
    HuntMarksDaily,
    HuntMarksWeekly,
    JumboCactpot,
    Levequest,
    MiniCactpot,
    TreasureMap,
    WondrousTails,
    ChallengeLog,
    NormalRaids,
    AllianceRaids,
    UnrealTrial,
    GrandCompanySupply,
    GrandCompanyProvision,
    MaskedCarnivale,
    GrandCompanySquadron
}

public static class ModuleNameExtensions
{
    public static string GetTranslatedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.BeastTribe => Strings.TribalQuests_Label,
            ModuleName.CustomDelivery => Strings.CustomDelivery_Label,
            ModuleName.DomanEnclave => Strings.DomanEnclave_Label,
            ModuleName.DutyRoulette => Strings.DutyRoulette_Label,
            ModuleName.FashionReport => Strings.FashionReport_Label,
            ModuleName.HuntMarksDaily => Strings.HuntMarks_DailyLabel,
            ModuleName.HuntMarksWeekly => Strings.HuntMarks_WeeklyLabel,
            ModuleName.JumboCactpot => Strings.JumboCactpot_Label,
            ModuleName.Levequest => Strings.Levequest_Label,
            ModuleName.MiniCactpot => Strings.MiniCactpot_Label,
            ModuleName.TreasureMap => Strings.TreasureMap_Label,
            ModuleName.WondrousTails => Strings.WondrousTails_Label,
            ModuleName.ChallengeLog => Strings.ChallengeLog_Label,
            ModuleName.NormalRaids => Strings.Raids_NormalLabel,
            ModuleName.AllianceRaids => Strings.Raids_AllianceLabel,
            ModuleName.UnrealTrial => Strings.FauxHollows_Label,
            ModuleName.GrandCompanySupply => Strings.GrandCompany_SupplyLabel,
            ModuleName.GrandCompanyProvision => Strings.GrandCompany_ProvisioningLabel,
            ModuleName.MaskedCarnivale => Strings.MaskedCarnivale_Label,
            ModuleName.GrandCompanySquadron => Strings.GrandCompany_SquadronLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}