using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Misc;

namespace DailyDuty.Modules;

internal class ChallengeLogSettings : GenericSettings
{
    public Setting<bool> CommendationWarning = new(true);
    public Setting<bool> RouletteDungeonWarning = new(true);
    public Setting<bool> DungeonWarning = new(true);
}

internal class ChallengeLog : IModule
{
    public ModuleName Name => ModuleName.ChallengeLog;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static ChallengeLogSettings Settings => Service.ConfigurationManager.CharacterConfiguration.ChallengeLog;
    public GenericSettings GenericSettings => Settings;

    public ChallengeLog()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance
                .AddTitle(Strings.Config_Options)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.ChallengeLog_CommendationLabel, Settings.CommendationWarning)
                .AddConfigCheckbox(Strings.ChallengeLog_DungeonRouletteLabel, Settings.RouletteDungeonWarning)
                .AddConfigCheckbox(Strings.ChallengeLog_DungeonMasterLabel, Settings.DungeonWarning)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericStatus(this);

            var commendationStatus = ModuleLogicComponent.CommendationStatus() == ModuleStatus.Complete;
            var rouletteStatus = ModuleLogicComponent.DungeonRouletteStatus() == ModuleStatus.Complete;
            var dungeonMasterStatus = ModuleLogicComponent.DungeonMasterStatus() == ModuleStatus.Complete;
            
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

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;
        
        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            CommendationAddon.Instance.Show += CommendationOnShow;
            
            DutyFinderAddon.Instance.Show += DutyFinderOnShow;
        }

        public void Dispose()
        {
            CommendationAddon.Instance.Show -= CommendationOnShow;
            
            DutyFinderAddon.Instance.Show -= DutyFinderOnShow;
        }

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
        
        public string GetStatusMessage() => string.Empty;

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            // Nothing to do here
        }

        public ModuleStatus GetModuleStatus()
        {
            if (CommendationStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
            if (DungeonRouletteStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
            if (DungeonMasterStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;

            return ModuleStatus.Complete;
        }

        public static ModuleStatus CommendationStatus() => ContentsNote.Instance()->IsContentNoteComplete(25) ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        public static ModuleStatus DungeonRouletteStatus() => ContentsNote.Instance()->IsContentNoteComplete(1) ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        public static ModuleStatus DungeonMasterStatus()=> ContentsNote.Instance()->IsContentNoteComplete(2) ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.ChallengeLog_Label;

        public string GetLongTaskLabel() => Strings.ChallengeLog_Label;

    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);

        public DateTime GetNextReset() => Time.NextWeeklyReset();
    }
}