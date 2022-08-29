using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

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

    private readonly DutyFinderOverlay overlay = new();
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
        private readonly InfoBox notificationOptions = new();
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
                .AddAction(() =>
                {
                    var checkboxAction = Actions.GetConfigCheckboxAction;

                    foreach (var roulette in Settings.TrackedRoulettes)
                        checkboxAction(roulette.Roulette.GetTranslatedString(), roulette.Tracked, null).Invoke();
                })
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.DutyRoulette.ClickableLinkLabel)
                .AddString(Strings.Module.DutyRoulette.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            notificationOptions
                .AddTitle(Strings.Configuration.NotificationOptions)
                .AddConfigCheckbox(Strings.Configuration.OnLogin, Settings.NotifyOnLogin)
                .AddConfigCheckbox(Strings.Configuration.OnZoneChange, Settings.NotifyOnZoneChange)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        private readonly InfoBox status = new();
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

            var moduleStatus = logicModule.GetModuleStatus();

            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()
                    .AddRow(
                        Strings.Status.ModuleStatus,
                        moduleStatus.GetTranslatedString(),
                        secondColor: moduleStatus.GetStatusColor())
                .EndTable()
                .Draw();

            if (Settings.TrackedRoulettes.Any(roulette => roulette.Tracked.Value))
            {
                trackedDuties
                    .AddTitle(Strings.Module.DutyRoulette.RouletteStatus)
                    .BeginTable()
                    .AddRows(Settings.TrackedRoulettes
                        .Where(row => row.Tracked.Value)
                        .Select(row => row.GetInfoBoxTableRow()))

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
            
            if (Settings.HideExpertWhenCapped.Value)
                tomestoneStatus
                    .AddTitle(Strings.Module.DutyRoulette.ExpertTomestones)
                    .BeginTable()
                    .AddRow(
                        Strings.Module.DutyRoulette.ExpertTomestones,
                        $"{logicModule.GetCurrentLimitedTomestoneCount()} / {logicModule.CurrentLimitedTomestoneWeeklyCap}",
                        secondColor: logicModule.HasMaxWeeklyTomestones() ? Colors.Green : Colors.Orange)
                    .EndTable()
                    .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }

        private static AgentModule* AgentModule => Framework.Instance()->GetUiModule()->GetAgentModule();

        private readonly AgentInterface* agentContentsFinder = AgentModule->GetAgentByInternalId(AgentId.ContentsFinder);

        public readonly long CurrentLimitedTomestoneWeeklyCap;

        private delegate IntPtr OpenRouletteToDutyDelegate(AgentInterface* agent, byte a2, byte a3);
        private delegate byte IsRouletteIncompleteDelegate(AgentInterface* agent, byte a2);
        public delegate long GetCurrentLimitedTomestoneCountDelegate(byte a1 = 9);

        [Signature("48 83 EC 28 80 F9 09")]
        public readonly GetCurrentLimitedTomestoneCountDelegate GetCurrentLimitedTomestoneCount = null!;

        [Signature("48 83 EC 28 84 D2 75 07 32 C0", ScanType = ScanType.Text)]
        private readonly IsRouletteIncompleteDelegate isRouletteIncomplete = null!;

        [Signature("E9 ?? ?? ?? ?? 8B 93 ?? ?? ?? ?? 48 83 C4 20")]
        private readonly OpenRouletteToDutyDelegate openRouletteDuty = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 0C 48 8D 4C 24", ScanType = ScanType.StaticAddress)]
        private readonly AgentInterface* rouletteBasePointer = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            DalamudLinkPayload =
                Service.PayloadManager.AddChatLink(ChatPayloads.OpenDutyFinder, OpenRouletteDutyFinder);
            CurrentLimitedTomestoneWeeklyCap = GetWeeklyTomestomeLimit();

            Service.Framework.Update += FrameworkOnUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;
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

        private void FrameworkOnUpdate(Dalamud.Game.Framework framework)
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

        public RouletteState GetRouletteState(RouletteType roulette)
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

        private void OpenRouletteDutyFinder(uint arg1, SeString arg2)
        {
            openRouletteDuty(agentContentsFinder, GetFirstMissingRoulette(), 0);
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
                .Select(roulette => roulette.Roulette.GetTranslatedString());

            return string.Join("\n", incompleteTasks);
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


    internal unsafe class DutyFinderOverlay : IDisposable
    {
        [Signature("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 30 48 8B 4B 10",
            DetourName = nameof(ContentsFinder_Show))]
        private readonly Hook<AgentShow>? contentsFinderShowHook = null;

        private record DutyFinderSearchResult(string SearchKey, uint Value);
        private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();

        private bool defaultColorSaved;

        private Hook<AddonDraw>? onDrawHook;
        private Hook<AddonFinalize>? onFinalizeHook;
        private Hook<AddonOnRefresh>? onRefreshHook;
        private ByteColor userDefaultTextColor;

        public DutyFinderOverlay()
        {
            SignatureHelper.Initialise(this);

            contentsFinderShowHook?.Enable();

            var rouletteData = Service.DataManager.GetExcelSheet<ContentRoulette>()
                !.Where(cr => cr.Name != string.Empty);

            foreach (var cr in rouletteData)
            {
                var simplifiedString = Regex.Replace(cr.Category.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

                dutyRouletteDuties.Add(new DutyFinderSearchResult(simplifiedString, cr.RowId));
            }
        }

        private IEnumerable<TrackedRoulette> DutyRoulettes => Settings.TrackedRoulettes;

        public void Dispose()
        {
            contentsFinderShowHook?.Dispose();

            onDrawHook?.Dispose();
            onRefreshHook?.Dispose();

            var frameworkInstance = Framework.Instance();
            var contentsFinderAgent =
                frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinder);
            if (contentsFinderAgent->IsAgentActive())
            {
                var addonPointer = GetContentsFinderPointer();
                ResetDefaultTextColor(addonPointer);
            }
        }

        private void* ContentsFinder_Show(void* a1)
        {
            var result = contentsFinderShowHook!.Original(a1);

            try
            {
                if (Settings.OverlayEnabled.Value)
                {
                    var frameworkInstance = Framework.Instance();
                    var contentsFinderAgent =
                        frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(
                            AgentId.ContentsFinder);

                    if (contentsFinderAgent->IsAgentActive())
                    {
                        var addonContentsFinder = GetContentsFinderPointer();

                        if (addonContentsFinder == null) return result;

                        var drawAddress = addonContentsFinder->AtkEventListener.vfunc[41];
                        var onRefreshAddress = addonContentsFinder->AtkEventListener.vfunc[48];
                        var finalizeAddress = addonContentsFinder->AtkEventListener.vfunc[39];

                        onDrawHook ??= Hook<AddonDraw>.FromAddress(new IntPtr(drawAddress), ContentsFinder_Draw);
                        onRefreshHook ??=
                            Hook<AddonOnRefresh>.FromAddress(new IntPtr(onRefreshAddress), ContentsFinder_OnRefresh);
                        onFinalizeHook ??=
                            Hook<AddonFinalize>.FromAddress(new IntPtr(finalizeAddress), ContentsFinder_Finalize);

                        onDrawHook.Enable();
                        onFinalizeHook.Enable();
                        onRefreshHook.Enable();

                        contentsFinderShowHook!.Disable();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something went wrong when the Duty Finder was opened");
            }

            return result;
        }

        private byte ContentsFinder_OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
        {
            var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

            try
            {
                if (Settings.Enabled.Value && Settings.OverlayEnabled.Value)
                {
                    if (IsTabSelected(0) == false)
                        ResetDefaultTextColor(atkUnitBase);
                    else
                        SetRouletteColors(atkUnitBase);
                }
                else
                {
                    ResetDefaultTextColor(atkUnitBase);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something when wrong on Duty Finder Tab Change or Duty Finder Refresh");
            }

            return result;
        }

        private void ContentsFinder_Draw(AtkUnitBase* atkUnitBase)
        {
            onDrawHook!.Original(atkUnitBase);
            if (atkUnitBase == null) return;

            try
            {
                if (defaultColorSaved == false)
                {
                    var textNode = GetListItemTextNode(atkUnitBase, 6);

                    if (textNode == null) return;

                    userDefaultTextColor = textNode->TextColor;
                    defaultColorSaved = true;
                }

                if (Settings.Enabled.Value && Settings.OverlayEnabled.Value)
                {
                    if (IsTabSelected(0) == false)
                        ResetDefaultTextColor(atkUnitBase);
                    else
                        SetRouletteColors(atkUnitBase);
                }
                else
                {
                    ResetDefaultTextColor(atkUnitBase);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something when wrong on Duty Finder Draw");
            }
        }

        private void* ContentsFinder_Finalize(AtkUnitBase* atkUnitBase)
        {
            if (atkUnitBase == null) return null;

            try
            {
                ResetDefaultTextColor(atkUnitBase);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something when wrong on Duty Finder Close");
            }

            return onFinalizeHook!.Original(atkUnitBase);
        }

        //
        //  Implementation
        //

        private void ResetDefaultTextColor(AtkUnitBase* rootNode)
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint) i;

                var textNode = GetListItemTextNode(rootNode, id);

                textNode->TextColor = userDefaultTextColor;
            }
        }

        private void SetRouletteColors(AtkUnitBase* rootNode)
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint) i;

                var rouletteState = IsRouletteDuty(rootNode, id);

                if (rouletteState != null)
                {
                    var textNode = GetListItemTextNode(rootNode, id);

                    if (rouletteState is {Tracked.Value: true, State: RouletteState.Complete})
                    {
                        textNode->TextColor.R = (byte) (Settings.CompleteColor.Value.X * 255);
                        textNode->TextColor.G = (byte) (Settings.CompleteColor.Value.Y * 255);
                        textNode->TextColor.B = (byte) (Settings.CompleteColor.Value.Z * 255);
                        textNode->TextColor.A = (byte) (Settings.CompleteColor.Value.W * 255);
                    }
                    else if (rouletteState is {Tracked.Value: true, State: RouletteState.Incomplete})
                    {
                        textNode->TextColor.R = (byte) (Settings.IncompleteColor.Value.X * 255);
                        textNode->TextColor.G = (byte) (Settings.IncompleteColor.Value.Y * 255);
                        textNode->TextColor.B = (byte) (Settings.IncompleteColor.Value.Z * 255);
                        textNode->TextColor.A = (byte) (Settings.IncompleteColor.Value.W * 255);
                    }
                    else if (rouletteState is {Tracked.Value: true, State: RouletteState.Overriden})
                    {
                        textNode->TextColor.R = (byte) (Settings.OverrideColor.Value.X * 255);
                        textNode->TextColor.G = (byte) (Settings.OverrideColor.Value.Y * 255);
                        textNode->TextColor.B = (byte) (Settings.OverrideColor.Value.Z * 255);
                        textNode->TextColor.A = (byte) (Settings.OverrideColor.Value.W * 255);
                    }
                    else
                    {
                        textNode->TextColor = userDefaultTextColor;
                    }
                }
            }
        }

        private TrackedRoulette? IsRouletteDuty(AtkUnitBase* rootNode, uint id)
        {
            var textNode = GetListItemTextNode(rootNode, id);

            var nodeString = textNode->NodeText.ToString().ToLower();
            var nodeRegexString = Regex.Replace(nodeString, "[^\\p{L}\\p{N}]", "");

            foreach (var result in dutyRouletteDuties)
                if (result.SearchKey == nodeRegexString)
                {
                    var trackedRoulette = DutyRoulettes
                        .Where(duty => (uint) duty.Roulette == result.Value)
                        .FirstOrDefault();

                    return trackedRoulette;
                }

            return null;
        }

        private AtkUnitBase* GetContentsFinderPointer()
        {
            return (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinder", 1);
        }

        private AtkComponentNode* GetListItemNode(AtkUnitBase* rootNode, uint nodeID)
        {
            var treeListBaseNode = Node.GetComponentNode(rootNode, 52);

            if (treeListBaseNode == null) return null;

            var listItemNode = Node.GetNodeByID<AtkComponentNode>(treeListBaseNode, nodeID);

            return listItemNode;
        }

        private AtkTextNode* GetTextNode(AtkComponentNode* listItemNode)
        {
            return Node.GetNodeByID<AtkTextNode>(listItemNode, 5);
        }

        private AtkTextNode* GetListItemTextNode(AtkUnitBase* rootNode, uint id)
        {
            var listItemNode = GetListItemNode(rootNode, id);

            if (listItemNode == null) return null;

            var textNode = GetTextNode(listItemNode);

            return textNode;
        }

        private bool IsTabSelected(uint tab)
        {
            var tabNodeId = 41 + tab;

            var baseNode = GetContentsFinderPointer();
            if (baseNode == null) return false;

            var specificTab = baseNode->GetNodeById(tabNodeId);
            if (specificTab == null) return false;

            var tabComponentNode = specificTab->GetAsAtkComponentNode();
            if (tabComponentNode == null) return false;

            var targetResNode = Node.GetNodeByID<AtkResNode>(tabComponentNode, 5);

            return targetResNode->AddRed > 16;
        }

        private delegate void* AgentShow(void* a1);

        private delegate void AddonDraw(AtkUnitBase* atkUnitBase);

        private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);

        private delegate void* AddonFinalize(AtkUnitBase* atkUnitBase);
    }
}