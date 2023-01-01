using System;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.DataStructures.HuntMarks;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class HuntMarksWeeklySettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}

internal class HuntMarksWeekly : IModule
{
    public ModuleName Name => ModuleName.HuntMarksWeekly;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static HuntMarksWeeklySettings Settings => Service.ConfigurationManager.CharacterConfiguration.HuntMarksWeekly;
    public GenericSettings GenericSettings => Settings;

    public HuntMarksWeekly()
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
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance
                .AddTitle(Strings.Module.HuntMarks.TrackedHunts)
                .AddList(Settings.TrackedHunts)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
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

            if (Settings.TrackedHunts.Any(hunt => hunt.Tracked))
            {
                InfoBox.Instance
                    .AddTitle(Strings.Module.HuntMarks.TrackedHuntsStatus)
                    .BeginTable()
                    .AddRows(Settings.TrackedHunts.Where(row => row.Tracked))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Module.HuntMarks.TrackedHuntsStatus)
                    .AddString(Strings.Module.HuntMarks.NoHuntsTracked, Colors.Orange)
                    .Draw();
            }

            InfoBox.Instance
                .AddTitle(Strings.Module.HuntMarks.ForceComplete, out var innerWidth)
                .AddString(Strings.Module.HuntMarks.ForceCompleteHelp, Colors.Orange)
                .AddDummy(20.0f)
                .AddStringCentered(Strings.Module.HuntMarks.NoUndo, Colors.Orange)
                .AddDisabledButton(Strings.UserInterface.Timers.Reset, () => 
                { 
                    foreach (var element in Settings.TrackedHunts)
                    {
                        element.State = TrackedHuntState.Killed;
                    }
                    Service.ConfigurationManager.Save();
                }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.Module.Raids.RegenerateTooltip, innerWidth)
                .Draw();
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;
        
        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
        }

        private void OnFrameworkUpdate(Framework framework)
        {
            if (!Service.ConfigurationManager.CharacterDataLoaded) return;

            foreach (var hunt in Settings.TrackedHunts)
            {
                UpdateState(hunt);
            }
        }

        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Module.HuntMarks.HuntsRemaining}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.State = TrackedHuntState.Unobtained;
            }
        }

        public ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private static void UpdateState(TrackedHunt hunt)
        {
            var data = HuntMarkData.Instance.GetHuntData(hunt.HuntType);

            switch (hunt.State)
            {
                case TrackedHuntState.Unobtained when data.Obtained:
                    hunt.State = TrackedHuntState.Obtained;
                    Service.ConfigurationManager.Save();
                    break;

                case TrackedHuntState.Obtained when data is { Obtained: false, IsCompleted: false }:
                    hunt.State = TrackedHuntState.Unobtained;
                    Service.ConfigurationManager.Save();
                    break;

                case TrackedHuntState.Obtained when data.IsCompleted:
                    hunt.State = TrackedHuntState.Killed;
                    Service.ConfigurationManager.Save();
                    break;
            }
        }

        private static int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => true;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.HuntMarks.WeeklyLabel;

        public string GetLongTaskLabel()
        {
            var strings = Settings.TrackedHunts
                .Where(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed)
                .Select(hunt => hunt.HuntType.GetLabel())
                .ToList();

            return strings.Any() ? string.Join("\n", strings) : Strings.Module.HuntMarks.WeeklyLabel;
        }
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