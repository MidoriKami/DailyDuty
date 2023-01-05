using System;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
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

internal class DomanEnclaveSettings : GenericSettings
{
    public int DonatedThisWeek;
    public int WeeklyAllowance;

    public Setting<bool> EnableClickableLink = new(true);
}

internal class DomanEnclave : IModule
{
    public ModuleName Name => ModuleName.DomanEnclave;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static DomanEnclaveSettings Settings => Service.ConfigurationManager.CharacterConfiguration.DomanEnclave;
    public GenericSettings GenericSettings => Settings;

    public DomanEnclave()
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
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.DomanEnclave_ClickableLink)
                .AddConfigCheckbox(Strings.Common_ClickableLink, Settings.EnableClickableLink)
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
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            InfoBox.Instance.DrawGenericStatus(this);
            
            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.DomanEnclave_BudgetRemaining)
                .AddString(ModuleLogicComponent.GetRemainingBudget().ToString(), ModuleLogicComponent.GetRemainingBudget() == 0 ? Colors.Green : Colors.Orange)
                .EndRow()
                .BeginRow()
                .AddString(Strings.DomanEnclave_CurrentAllowance)
                .AddString(Settings.WeeklyAllowance.ToString())
                .EndRow()
                .EndTable()
                .Draw();
                
            if (moduleStatus == ModuleStatus.Unknown)
            {
                InfoBox.Instance
                    .AddTitle(Strings.DomanEnclave_StatusUnknown)
                    .AddString(Strings.DomanEnclave_StatusUnknown_Info, Colors.Orange)
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

        private delegate DomanEnclaveStruct* GetDataDelegate();

        [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 09 0F B6 B8")]
        private readonly GetDataDelegate getDomanEnclaveStruct = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = TeleportManager.Instance.GetPayload(TeleportLocation.DomanEnclave);
            
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
            if (!DataAvailable()) return;

            UpdateWeeklyAllowance();
            UpdateDonatedThisWeek();
        }

        public string GetStatusMessage()
        {
            if (GetModuleStatus() == ModuleStatus.Unknown) return Strings.DomanEnclave_StatusUnknown;

            return $"{GetRemainingBudget()} {Strings.DomanEnclave_GilRemaining}";
        }

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset() => Settings.DonatedThisWeek = 0;

        public ModuleStatus GetModuleStatus()
        {
            if (!ModuleInitialized()) return ModuleStatus.Unknown;

            return GetRemainingBudget() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        }

        private void UpdateWeeklyAllowance()
        {
            var allowance = GetWeeklyAllowance();

            if (Settings.WeeklyAllowance != allowance)
            {
                Settings.WeeklyAllowance = allowance;
                Service.ConfigurationManager.Save();
            }
        }
        private void UpdateDonatedThisWeek()
        {
            var donatedThisWeek = GetDonatedThisWeek();

            if (Settings.DonatedThisWeek != donatedThisWeek)
            {
                Settings.DonatedThisWeek = donatedThisWeek;
                Service.ConfigurationManager.Save();
            }
        }

        public static int GetRemainingBudget() => Settings.WeeklyAllowance - Settings.DonatedThisWeek;
        private ushort GetDonatedThisWeek() => getDomanEnclaveStruct()->Donated;
        private ushort GetWeeklyAllowance() => getDomanEnclaveStruct()->Allowance;
        private bool DataAvailable() => GetWeeklyAllowance() != 0;
        private static bool ModuleInitialized() => Settings.WeeklyAllowance != 0;
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

        public string GetShortTaskLabel() => Strings.DomanEnclave_Label;

        public string GetLongTaskLabel()  => Strings.DomanEnclave_Label;
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