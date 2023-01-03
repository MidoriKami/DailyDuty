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
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class HuntMarksDailySettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}

internal class HuntMarksDaily : IModule
{
    public ModuleName Name => ModuleName.HuntMarksDaily;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static HuntMarksDailySettings Settings => Service.ConfigurationManager.CharacterConfiguration.HuntMarksDaily;
    public GenericSettings GenericSettings => Settings;

    public HuntMarksDaily()
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
                .AddTitle(Strings.HuntMarks_Tracked)
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
                    .AddTitle(Strings.HuntMarks_Status)
                    .BeginTable(0.60f)
                    .AddRows(Settings.TrackedHunts.Where(row => row.Tracked))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.HuntMarks_Status)
                    .AddString(Strings.HuntMarks_NothingTracked, Colors.Orange)
                    .Draw();
            }
            
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

        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.HuntMarks_Remaining}";

        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.State = TrackedHuntState.Unobtained;
            }
        }

        public ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private static int GetIncompleteCount() => Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Daily;
        public bool HasLongLabel => true;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.HuntMarks_DailyLabel;

        public string GetLongTaskLabel()
        {
            var strings = Settings.TrackedHunts
                .Where(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed)
                .Select(hunt => hunt.HuntType.GetLabel())
                .ToList();

            return strings.Any() ? string.Join("\n", strings) : Strings.HuntMarks_DailyLabel;
        }
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}