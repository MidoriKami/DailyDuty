using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class FauxHollowsSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(true);
    public Setting<bool> IncludeRetelling = new(true);
    public int FauxHollowsCompleted;
}

internal class FauxHollows : IModule
{
    public ModuleName Name => ModuleName.UnrealTrial;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static FauxHollowsSettings Settings => Service.ConfigurationManager.CharacterConfiguration.FauxHollows;
    public GenericSettings GenericSettings => Settings;

    public FauxHollows()
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
                .AddTitle(Strings.FauxHollows_Retelling)
                .AddConfigCheckbox(Strings.FauxHollows_Retelling, Settings.IncludeRetelling, Strings.FauxHollows_Retelling_Info)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.PartyFinder_ClickableLink)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
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
            
            InfoBox.Instance.DrawGenericStatus(this);

            InfoBox.Instance
                .AddTitle(Strings.Status_ModuleData)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Common_Completions)
                .AddString($"{Settings.FauxHollowsCompleted} / {GetRequiredCompletionCount()}", logicModule.GetModuleStatus().GetStatusColor())
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.HuntMarks_ForceComplete, out var innerWidth)
                .AddStringCentered(Strings.HuntMarks_ForceComplete_Info, innerWidth, Colors.Orange)
                .AddDummy(20.0f)
                .AddStringCentered(Strings.HuntMarks_ForceComplete_Warning, innerWidth, Colors.Orange)
                .AddDisabledButton(Strings.Common_Reset, () => { 
                    Settings.FauxHollowsCompleted = GetRequiredCompletionCount();
                    Service.ConfigurationManager.Save();
                }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.DisabledButton_Hover, innerWidth)
                .Draw();
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }

        private static int GetRequiredCompletionCount()
        {
            return Settings.IncludeRetelling ? 2 : 1;
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = ChatPayloadManager.Instance.AddChatLink(ChatPayloads.OpenPartyFinder, OpenPartyFinder);

            WeeklyPuzzleAddon.Instance.Show += OnShow;
        }
        
        public void Dispose()
        {
            WeeklyPuzzleAddon.Instance.Show -= OnShow;

        }

        private void OnShow(object? sender, IntPtr e)
        {
            if (!Settings.Enabled) return;

            Settings.FauxHollowsCompleted += 1;
            Service.ConfigurationManager.Save();
        }

        private void OpenPartyFinder(uint arg1, SeString arg2)
        {
            Service.ChatManager.SendCommandUnsafe("partyfinder");
        }

        public string GetStatusMessage() => $"{Strings.FauxHollows_TrialAvailable}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            Settings.FauxHollowsCompleted = 0;
        }

        public ModuleStatus GetModuleStatus() => Settings.FauxHollowsCompleted >= GetRequiredCompletionCount() ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private static int GetRequiredCompletionCount()
        {
            return Settings.IncludeRetelling ? 2 : 1;
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

        public string GetShortTaskLabel() => Strings.FauxHollows_Label;

        public string GetLongTaskLabel() => Strings.FauxHollows_Label;
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