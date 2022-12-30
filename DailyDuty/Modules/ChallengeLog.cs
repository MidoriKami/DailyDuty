using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

internal class ChallengeLogSettings : GenericSettings
{
    public int Commendations;
    public int RouletteDungeons;
    public int DungeonMaster;

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
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Module.ChallengeLog.CommendationLabel, Settings.CommendationWarning)
                .AddConfigCheckbox(Strings.Module.ChallengeLog.DungeonRouletteLabel, Settings.RouletteDungeonWarning)
                .AddConfigCheckbox(Strings.Module.ChallengeLog.DungeonMasterLabel, Settings.DungeonWarning)
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
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;
           
            InfoBox.Instance.DrawGenericStatus(this);

            InfoBox.Instance
                .AddTitle(Strings.Module.ChallengeLog.Battle)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Module.ChallengeLog.Commendations)
                .AddString($"{Settings.Commendations} / 5", logicModule.CommendationStatus().GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.ChallengeLog.DungeonRoulette)
                .AddString($"{Settings.RouletteDungeons} / 3", logicModule.DungeonRouletteStatus().GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.ChallengeLog.DungeonMaster)
                .AddString($"{Settings.DungeonMaster} / 5", logicModule.DungeonMasterStatus().GetStatusColor())
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

        private delegate void* ChallengeLogNetworkData(void* a1, int a2, int* a3);
        [Signature("40 55 57 41 56 48 83 EC 20 0F B6 41 1C", DetourName = nameof(ProcessNetworkPacket))]
        private readonly Hook<ChallengeLogNetworkData>? refreshChallengeLogHook = null;

        private enum DungeonDutyRouletteState
        {
            NotDungeonDuty,
            DutyPopped,
            DutyCommencing,
            EnteredDuty,
            DutyCompleted
        }

        private DungeonDutyRouletteState dutyState = DungeonDutyRouletteState.NotDungeonDuty;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            SignatureHelper.Initialise(this);

            refreshChallengeLogHook?.Enable();

            Service.ClientState.CfPop += OnContentFinderPop;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            var commendationAddon = Service.AddonManager.Get<CommendationAddon>();
            commendationAddon.ReceiveEvent += CommendationOnReceiveEvent;
            commendationAddon.Show += CommendationOnShow;

            var dutyFinderAddon = Service.AddonManager.Get<DutyFinderAddon>();
            dutyFinderAddon.ReceiveEvent += DutyFinderOnReceiveEvent;
            dutyFinderAddon.Show += DutyFinderOnShow;

            var dutyEventAddon = Service.AddonManager.Get<DutyEventAddon>();
            dutyEventAddon.DutyCompleted += OnDutyCompleted;
        }

        public void Dispose()
        {
            refreshChallengeLogHook?.Dispose();

            Service.ClientState.CfPop -= OnContentFinderPop;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;

            var commendationAddon = Service.AddonManager.Get<CommendationAddon>();
            commendationAddon.ReceiveEvent -= CommendationOnReceiveEvent;
            commendationAddon.Show -= CommendationOnShow;

            var dutyFinderAddon = Service.AddonManager.Get<DutyFinderAddon>();
            dutyFinderAddon.ReceiveEvent -= DutyFinderOnReceiveEvent;
            dutyFinderAddon.Show -= DutyFinderOnShow;

            var dutyEventAddon = Service.AddonManager.Get<DutyEventAddon>();
            dutyEventAddon.DutyCompleted -= OnDutyCompleted;
        }

        private void DutyFinderOnShow(object? sender, IntPtr e)
        {
            if (!Settings.Enabled.Value) return;

            if (Settings.RouletteDungeonWarning.Value && Settings.RouletteDungeons < 3)
            {
                Chat.Print(Strings.Module.ChallengeLog.Label, $"{3 - Settings.RouletteDungeons} {Strings.Module.ChallengeLog.DungeonRoulettesRemaining}");
            }

            if (Settings.DungeonWarning.Value && Settings.DungeonMaster < 5)
            {
                Chat.Print(Strings.Module.ChallengeLog.Label, $"{5 - Settings.DungeonMaster} {Strings.Module.ChallengeLog.DungeonMasterRemaining}");
            }
        }

        private void OnContentFinderPop(object? sender, ContentFinderCondition queuedDuty) => dutyState = queuedDuty.RowId == 0 ? DungeonDutyRouletteState.DutyPopped : DungeonDutyRouletteState.NotDungeonDuty;

        private void DutyFinderOnReceiveEvent(object? sender, ReceiveEventArgs e)
        {
            // Commence / Wait / Withdraw dialogue
            if (e.SenderID != 2) return;

            // 8 = Commence button was pushed
            if (e.EventArgs[0].Int == 8 && dutyState == DungeonDutyRouletteState.DutyPopped)
            {
                dutyState = DungeonDutyRouletteState.DutyCommencing;
            }
        }

        private void OnDutyCompleted(object? sender, uint territory)
        {
            if (dutyState == DungeonDutyRouletteState.EnteredDuty)
            {
                dutyState = DungeonDutyRouletteState.DutyCompleted;
            }

            if (GetDutyType(Service.ClientState.TerritoryType) == 2 && Settings.DungeonMaster < 5)
            {
                Settings.DungeonMaster += 1;
                Service.ConfigurationManager.Save();
            }
        }
        
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            switch (dutyState)
            {
                case DungeonDutyRouletteState.DutyCommencing:
                    // 2 = Dungeons
                    if (GetDutyType(e) == 2)
                    {
                        dutyState = DungeonDutyRouletteState.EnteredDuty;
                    }
                    break;

                case DungeonDutyRouletteState.DutyCompleted:
                    if (Settings.RouletteDungeons < 3)
                    {
                        Settings.RouletteDungeons += 1;
                        Service.ConfigurationManager.Save();
                    }
                    
                    dutyState = DungeonDutyRouletteState.NotDungeonDuty;
                    break;

                default:
                    dutyState = DungeonDutyRouletteState.NotDungeonDuty;
                    break;
            }
        }

        private void CommendationOnReceiveEvent(object? sender, ReceiveEventArgs e)
        {
            if (e.EventArgsCount == 2 && Settings.Commendations < 5)
            {
                Settings.Commendations += 1;
                Service.ConfigurationManager.Save();
            }
        }

        private void CommendationOnShow(object? sender, IntPtr e)
        {
            if (!Settings.Enabled.Value) return;

            if (Settings.CommendationWarning.Value && Settings.Commendations < 5)
            {
                Chat.Print(Strings.Module.ChallengeLog.Label, $"{5 - Settings.Commendations} {Strings.Module.ChallengeLog.CommendationsRemaining}");
            }
        }

        private void* ProcessNetworkPacket(void* a1, int tab, int* a3)
        {
            try
            {
                switch (tab)
                {
                    // Battle Tab
                    case 1:
                        var data = (ChallengeLogStruct.Battles*) a3;
                        Settings.Commendations = data->Commendations;
                        Settings.RouletteDungeons = data->DungeonRoulette;
                        Settings.DungeonMaster = data->DungeonMaster;
                        Service.ConfigurationManager.Save();
                        break;
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "Unable to Refresh Challenge Log Data");
            }

            return refreshChallengeLogHook!.Original(a1, tab, a3);
        }

        private uint? GetDutyType(uint territoryType)
        {
            var territoryInfo = LuminaCache<TerritoryType>.Instance.GetRow(territoryType);
            var contentFinderInfo = territoryInfo?.ContentFinderCondition.Value;
            var contentType = contentFinderInfo?.ContentType.Value;

            return contentType?.RowId;
        }

        public string GetStatusMessage() => string.Empty;

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            Settings.Commendations = 0;
            Settings.RouletteDungeons = 0;
        }

        public ModuleStatus GetModuleStatus()
        {
            if (CommendationStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
            if (DungeonRouletteStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;
            if (DungeonMasterStatus() != ModuleStatus.Complete) return ModuleStatus.Incomplete;

            return ModuleStatus.Complete;
        }

        public ModuleStatus CommendationStatus()
        {
            if (Settings.CommendationWarning.Value && Settings.Commendations < 5) return ModuleStatus.Incomplete;

            return ModuleStatus.Complete;
        }

        public ModuleStatus DungeonRouletteStatus()
        {
            if (Settings.RouletteDungeonWarning.Value && Settings.RouletteDungeons < 3) return ModuleStatus.Incomplete;

            return ModuleStatus.Complete;
        }

        public ModuleStatus DungeonMasterStatus()
        {
            if (Settings.DungeonWarning.Value && Settings.DungeonMaster < 5) return ModuleStatus.Incomplete;

            return ModuleStatus.Complete;
        }
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

        public string GetShortTaskLabel() => Strings.Module.ChallengeLog.Label;

        public string GetLongTaskLabel() => Strings.Module.ChallengeLog.Label;

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