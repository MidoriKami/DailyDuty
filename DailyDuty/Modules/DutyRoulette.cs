using System;
using System.Linq;
using System.Numerics;
using DailyDuty.AddonOverlays;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Misc;
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
        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public void Draw()
        {
            InfoBox.Instance
                .AddTitle(Strings.Config_Options)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.DutyRoulette_ExpertFeature, Settings.HideExpertWhenCapped, Strings.DutyRoulette_ExpertFeature_Info)
                .AddConfigCheckbox(Strings.DutyRoulette_CompleteWhenCapped, Settings.CompleteWhenCapped, Strings.DutyRoulette_CompleteWhenCapped_Info)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.DutyFinder_Overlay)
                .AddConfigCheckbox(Strings.DutyFinder_Overlay, Settings.OverlayEnabled)
                .AddConfigColor(Strings.DutyRoulette_DutyComplete, Strings.Common_Default, Settings.CompleteColor, Colors.Green)
                .AddConfigColor(Strings.DutyRoulette_DutyIncomplete, Strings.Common_Default, Settings.IncompleteColor, Colors.Red)
                .AddConfigColor(Strings.DutyRoulette_Override, Strings.Common_Default, Settings.OverrideColor, Colors.Orange)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.DutyRoulette_RouletteSelection)
                .AddList(Settings.TrackedRoulettes)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.DutyFinder_ClickableLink)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public IModule ParentModule { get; }

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            InfoBox.Instance.DrawGenericStatus(this);

            if (Settings.TrackedRoulettes.Any(roulette => roulette.Tracked))
            {
                InfoBox.Instance
                    .AddTitle(Strings.DutyRoulette_Status)
                    .BeginTable()
                    .AddDataRows(Settings.TrackedRoulettes.Where(row => row.Tracked))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.DutyRoulette_Status, out var innerWidth)
                    .AddStringCentered(Strings.DutyRoulette_NothingTracked, innerWidth, Colors.Orange)
                    .Draw();
            }

            if (Settings.HideExpertWhenCapped || Settings.CompleteWhenCapped)
            {
                InfoBox.Instance
                    .AddTitle(Strings.DutyRoulette_ExpertTomestones)
                    .BeginTable()
                    .BeginRow()
                    .AddString(Strings.DutyRoulette_ExpertTomestones)
                    .AddString($"{logicModule.GetCurrentLimitedTomestoneCount()} / {logicModule.CurrentLimitedTomestoneWeeklyCap}", logicModule.HasMaxWeeklyTomestones() ? Colors.Green : Colors.Orange)
                    .EndRow()
                    .EndTable()
                    .Draw();
            }
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink;

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

            DalamudLinkPayload = ChatPayloadManager.Instance.AddChatLink(ChatPayloads.DutyRouletteDutyFinder, OpenDutyFinder);
            CurrentLimitedTomestoneWeeklyCap = GetWeeklyTomestomeLimit();

            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
        }

        public string GetStatusMessage()
        {
            return $"{RemainingRoulettesCount()} {Strings.DutyRoulette_Remaining}";
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
            if (roulette == RouletteType.Expert && Settings.HideExpertWhenCapped)
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

        private static int GetWeeklyTomestomeLimit()
        {
            return LuminaCache<TomestonesItem>.Instance
                .Select(t => t.Tomestones.Value)
                .OfType<Tomestones>()
                .Where(t => t.WeeklyLimit > 0)
                .Max(t => t.WeeklyLimit);
        }

        private int RemainingRoulettesCount()
        {
            if (Settings.CompleteWhenCapped && HasMaxWeeklyTomestones())
            {
                return 0;
            }

            return Settings.TrackedRoulettes
                .Where(r => r.Tracked)
                .Count(r => r.State == RouletteState.Incomplete);
        }

        private static byte GetFirstMissingRoulette()
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

        public string GetShortTaskLabel() => Strings.DutyRoulette_Label;

        public string GetLongTaskLabel()
        {
            var incompleteTasks = Settings.TrackedRoulettes
                .Where(roulette => roulette.Tracked && roulette.State == RouletteState.Incomplete)
                .Select(roulette => roulette.Roulette.GetTranslatedString())
                .ToList();

            return incompleteTasks.Any() ? string.Join("\n", incompleteTasks) : Strings.DutyRoulette_Label;
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