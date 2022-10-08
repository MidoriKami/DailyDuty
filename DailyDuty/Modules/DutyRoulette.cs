using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Addons.Overlays;
using DailyDuty.Configuration.Components;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class DutyRouletteSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(false);
    public Setting<bool> HideExpertWhenCapped = new(false);
    public Setting<bool> CompleteWhenCapped = new(false);

    public Setting<bool> OverlayEnabled = new(true);
    public Setting<Vector4> CompleteColor = new(Colors.Green);
    public Setting<Vector4> IncompleteColor = new(Colors.Red);
    public Setting<Vector4> OverrideColor = new(Colors.Orange);

    public TrackedRoulette[] TrackedRoulettes =
    {
        new(RouletteType.Expert, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Level90, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Level50607080, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Leveling, new Setting<bool>(false), RouletteState.Incomplete), 
        new(RouletteType.Trials, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.MSQ, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Guildhest, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Alliance, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Normal, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Frontline, new Setting<bool>(false), RouletteState.Incomplete),
        new(RouletteType.Mentor, new Setting<bool>(false), RouletteState.Incomplete),
    };
}

internal class DutyRoulette : IModule
{
    private static DutyRouletteSettings Settings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    public ModuleName Name => ModuleName.DutyRoulette;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }
    public GenericSettings GenericSettings => Settings;

    private readonly DutyRouletteOverlay overlay = new();
    public DutyRoulette()
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
        overlay.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        private readonly InfoBox clickableLink = new();
        private readonly InfoBox dutyFinder = new();
        private readonly InfoBox options = new();
        private readonly InfoBox rouletteSelection = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public void Draw()
        {
            options
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Module.DutyRoulette.HideExpertWhenCapped, Settings.HideExpertWhenCapped, Strings.Module.DutyRoulette.HideExpertHelp)
                .AddConfigCheckbox(Strings.Module.DutyRoulette.CompleteWhenCapped, Settings.CompleteWhenCapped, Strings.Module.DutyRoulette.CompleteWhenCappedHelp)
                .Draw();

            dutyFinder
                .AddTitle(Strings.Module.DutyRoulette.Overlay)
                .AddConfigCheckbox(Strings.Module.DutyRoulette.Overlay, Settings.OverlayEnabled)
                .AddConfigColor(Strings.Module.DutyRoulette.DutyComplete, Settings.CompleteColor)
                .AddConfigColor(Strings.Module.DutyRoulette.DutyIncomplete, Settings.IncompleteColor)
                .AddConfigColor(Strings.Module.DutyRoulette.Override, Settings.OverrideColor)
                .Draw();

            rouletteSelection
                .AddTitle(Strings.Module.DutyRoulette.RouletteSelection)
                .AddList(Settings.TrackedRoulettes)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.DutyRoulette.ClickableLinkLabel)
                .AddString(Strings.Module.DutyRoulette.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        private readonly InfoBox trackedDuties = new();
        private readonly InfoBox tomestoneStatus = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            InfoBox.DrawGenericStatus(this);

            if (Settings.TrackedRoulettes.Any(roulette => roulette.Tracked.Value))
            {
                trackedDuties
                    .AddTitle(Strings.Module.DutyRoulette.RouletteStatus)
                    .BeginTable()
                    .AddRows(Settings.TrackedRoulettes.Where(row => row.Tracked.Value))
                    .EndTable()
                    .Draw();
            }
            else
            {
                trackedDuties
                    .AddTitle(Strings.Module.DutyRoulette.RouletteStatus)
                    .AddString(Strings.Module.DutyRoulette.NoRoulettesTracked, Colors.Orange)
                    .Draw();
            }

            if (Settings.HideExpertWhenCapped.Value || Settings.CompleteWhenCapped.Value)
            {
                tomestoneStatus
                    .AddTitle(Strings.Module.DutyRoulette.ExpertTomestones)
                    .BeginTable()
                    .BeginRow()
                    .AddString(Strings.Module.DutyRoulette.ExpertTomestones)
                    .AddString($"{logicModule.GetCurrentLimitedTomestoneCount()} / {logicModule.CurrentLimitedTomestoneWeeklyCap}", logicModule.HasMaxWeeklyTomestones() ? Colors.Green : Colors.Orange)
                    .EndRow()
                    .EndTable()
                    .Draw();
            }
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }

        public readonly long CurrentLimitedTomestoneWeeklyCap;

        private delegate byte IsRouletteIncompleteDelegate(AgentInterface* agent, byte a2);
        public delegate long GetCurrentLimitedTomestoneCountDelegate(byte a1 = 9);

        [Signature("48 83 EC 28 80 F9 09")]
        public readonly GetCurrentLimitedTomestoneCountDelegate GetCurrentLimitedTomestoneCount = null!;

        [Signature("48 83 EC 28 84 D2 75 07 32 C0", ScanType = ScanType.Text)]
        private readonly IsRouletteIncompleteDelegate isRouletteIncomplete = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 0C 48 8D 4C 24", ScanType = ScanType.StaticAddress)]
        private readonly AgentInterface* rouletteBasePointer = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            DalamudLinkPayload = Service.PayloadManager.AddChatLink(ChatPayloads.DutyRouletteDutyFinder, OpenDutyFinder);
            CurrentLimitedTomestoneWeeklyCap = GetWeeklyTomestomeLimit();

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
        }

        public string GetStatusMessage()
        {
            return $"{RemainingRoulettesCount()} {Strings.Module.DutyRoulette.Remaining}";
        }

        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset()
        {
            foreach (var task in Settings.TrackedRoulettes) task.State = RouletteState.Incomplete;
        }

        public ModuleStatus GetModuleStatus()
        {
            return RemainingRoulettesCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        }

        private void OnFrameworkUpdate(Dalamud.Game.Framework framework)
        {
            if (!Service.ConfigurationManager.CharacterDataLoaded) return;

            foreach (var trackedRoulette in Settings.TrackedRoulettes)
            {
                var rouletteStatus = GetRouletteState(trackedRoulette.Roulette);

                if (trackedRoulette.State != rouletteStatus)
                {
                    trackedRoulette.State = rouletteStatus;
                    Service.ConfigurationManager.Save();
                }
            }
        }

        private RouletteState GetRouletteState(RouletteType roulette)
        {
            if (roulette == RouletteType.Expert && Settings.HideExpertWhenCapped.Value)
            {
                if (HasMaxWeeklyTomestones())
                {
                    return RouletteState.Overriden;
                }
            }

            var isComplete = isRouletteIncomplete(rouletteBasePointer, (byte) roulette) == 0;

            return isComplete ? RouletteState.Complete : RouletteState.Incomplete;
        }

        public bool HasMaxWeeklyTomestones()
        {
            return GetCurrentLimitedTomestoneCount() == CurrentLimitedTomestoneWeeklyCap;
        }

        private void OpenDutyFinder(uint arg1, SeString arg2)
        {
            AgentContentsFinder.Instance()->OpenRouletteDuty(GetFirstMissingRoulette());
        }

        private int GetWeeklyTomestomeLimit()
        {
            return Service.DataManager
                .GetExcelSheet<TomestonesItem>()!
                .Select(t => t.Tomestones.Value)
                .OfType<Tomestones>()
                .Where(t => t.WeeklyLimit > 0)
                .Max(t => t.WeeklyLimit);
        }

        private int RemainingRoulettesCount()
        {
            if (Settings.CompleteWhenCapped.Value && HasMaxWeeklyTomestones())
            {
                return 0;
            }

            return Settings.TrackedRoulettes
                .Where(r => r.Tracked.Value)
                .Count(r => r.State == RouletteState.Incomplete);
        }

        private byte GetFirstMissingRoulette()
        {
            foreach (var trackedRoulette in Settings.TrackedRoulettes)
                if (trackedRoulette is {State: RouletteState.Incomplete, Tracked.Value: true})
                    return (byte) trackedRoulette.Roulette;

            return (byte) RouletteType.Leveling;
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Daily;
        public bool HasLongLabel => true;

        public string GetShortTaskLabel() => Strings.Module.DutyRoulette.Label;

        public string GetLongTaskLabel()
        {
            var incompleteTasks = Settings.TrackedRoulettes
                .Where(roulette => roulette.Tracked.Value && roulette.State == RouletteState.Incomplete)
                .Select(roulette => roulette.Roulette.GetTranslatedString())
                .ToList();

            return incompleteTasks.Any() ? string.Join("\n", incompleteTasks) : Strings.Module.DutyRoulette.Label;
        }
    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}