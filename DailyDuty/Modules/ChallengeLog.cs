using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.Modules;

public class ChallengeLogSettings : GenericSettings
{
    public Setting<bool> CommendationWarning = new(true);
    public Setting<bool> RouletteDungeonWarning = new(true);
    public Setting<bool> DungeonWarning = new(true);
}

public unsafe class ChallengeLog : AbstractModule
{
    public override ModuleName Name => ModuleName.ChallengeLog;
    public override CompletionType CompletionType => CompletionType.Weekly;
    
    private static ChallengeLogSettings Settings => Service.ConfigurationManager.CharacterConfiguration.ChallengeLog;
    public override GenericSettings GenericSettings => Settings;

    public ChallengeLog()
    {
        CommendationAddon.Instance.Show += CommendationOnShow;
        DutyFinderAddon.Instance.Show += DutyFinderOnShow;
    }
    
    public override void Dispose()
    {
        CommendationAddon.Instance.Show -= CommendationOnShow;
        DutyFinderAddon.Instance.Show -= DutyFinderOnShow;
    }
    
    public static ModuleStatus CommendationStatus() => ContentsNote.Instance()->IsContentNoteComplete(25) ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    public static ModuleStatus DungeonRouletteStatus() => ContentsNote.Instance()->IsContentNoteComplete(1) ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    public static ModuleStatus DungeonMasterStatus()=> ContentsNote.Instance()->IsContentNoteComplete(2) ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    private void DutyFinderOnShow(object? sender, nint e)
    {
        if (!Settings.Enabled) return;
        if (Settings.Suppressed) return;

        if (Settings.RouletteDungeonWarning && DungeonRouletteStatus() != ModuleStatus.Complete)
        {
            Chat.Print(Strings.ChallengeLog_Label, $"{Strings.ChallengeLog_DungeonRoulette} {Strings.Common_AllowancesAvailable}");
        }

        if (Settings.DungeonWarning && DungeonMasterStatus() != ModuleStatus.Complete)
        {
            Chat.Print(Strings.ChallengeLog_Label, $"{Strings.ChallengeLog_DungeonMaster} {Strings.Common_AllowancesAvailable}");
        }
    }

    private void CommendationOnShow(object? sender, nint e)
    {
        if (!Settings.Enabled) return;
        if (Settings.Suppressed) return;

        if (Settings.CommendationWarning && CommendationStatus() != ModuleStatus.Complete)
        {
            Chat.Print(Strings.ChallengeLog_Label, $"{Strings.ChallengeLog_Commendation} {Strings.Common_AllowancesAvailable}");
        }
    }
    
    public override ModuleStatus GetModuleStatus()
    {
        if (CommendationStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
        if (DungeonRouletteStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
        if (DungeonMasterStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;

        return ModuleStatus.Complete;
    }

    protected override void DrawConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Config_Options)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
            .AddConfigCheckbox(Strings.ChallengeLog_CommendationLabel, Settings.CommendationWarning)
            .AddConfigCheckbox(Strings.ChallengeLog_DungeonRouletteLabel, Settings.RouletteDungeonWarning)
            .AddConfigCheckbox(Strings.ChallengeLog_DungeonMasterLabel, Settings.DungeonWarning)
            .Draw();
    }
    
    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        var commendationStatus = CommendationStatus() == ModuleStatus.Complete;
        var rouletteStatus = DungeonRouletteStatus() == ModuleStatus.Complete;
        var dungeonMasterStatus = DungeonMasterStatus() == ModuleStatus.Complete;
            
        InfoBox.Instance
            .AddTitle(Strings.Common_Battle)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.ChallengeLog_Commendation)
            .AddString(commendationStatus ? Strings.Common_Complete : Strings.Common_Incomplete, commendationStatus ? Colors.Green : Colors.Orange)
            .EndRow()
            .BeginRow()
            .AddString(Strings.ChallengeLog_DungeonRoulette)
            .AddString(rouletteStatus ? Strings.Common_Complete : Strings.Common_Incomplete, rouletteStatus ? Colors.Green : Colors.Orange)
            .EndRow()
            .BeginRow()
            .AddString(Strings.ChallengeLog_DungeonMaster)
            .AddString(dungeonMasterStatus ? Strings.Common_Complete : Strings.Common_Incomplete, dungeonMasterStatus ? Colors.Green : Colors.Orange)
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}