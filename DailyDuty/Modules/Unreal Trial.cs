using System;
using System.Numerics;
using DailyDuty.Addons;
using DailyDuty.Configuration.Components;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Modules;

public class UnrealTrialSettings : GenericSettings
{
    public Setting<bool> EnableClickableLink = new(true);
    public Setting<bool> IncludeRetelling = new(true);
    public int FauxHollowsCompleted;
}

internal class UnrealTrial : IModule
{
    public ModuleName Name => ModuleName.UnrealTrial;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static UnrealTrialSettings Settings => Service.ConfigurationManager.CharacterConfiguration.UnrealTrial;
    public GenericSettings GenericSettings => Settings;

    public UnrealTrial()
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

        private readonly InfoBox options = new();
        private readonly InfoBox notificationOptions = new();
        private readonly InfoBox clickableLink = new();
        private readonly InfoBox completionCondition = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            options
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            completionCondition
                .AddTitle(Strings.Module.UnrealTrial.Retelling)
                .AddConfigCheckbox(Strings.Module.UnrealTrial.Retelling, Settings.IncludeRetelling, Strings.Module.UnrealTrial.RetellingHelp)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.UnrealTrial.ClickableLinkLabel)
                .AddString(Strings.Module.UnrealTrial.ClickableLink)
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
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox status = new();
        private readonly InfoBox target = new();
        private readonly InfoBox forceComplete = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }
        
        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .EndTable()
                .Draw();

            target
                .AddTitle(Strings.Common.Target)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Module.UnrealTrial.Completions)
                .AddString($"{Settings.FauxHollowsCompleted} / {GetRequiredCompletionCount()}", ParentModule.LogicComponent.GetModuleStatus().GetStatusColor())
                .EndRow()
                .EndTable()
                .Draw();

            forceComplete
                .AddTitle(Strings.Module.HuntMarks.ForceComplete)
                .AddAction(ForceCompleteButton)
                .Draw();
        }


        private void ForceCompleteButton()
        {
            var keys = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;

            ImGui.TextColored(Colors.Orange, Strings.Module.HuntMarks.ForceCompleteHelp);

            ImGuiHelpers.ScaledDummy(15.0f);

            var textSize = ImGui.CalcTextSize(Strings.Module.HuntMarks.NoUndo);
            var cursor = ImGui.GetCursorPos();
            var availableArea = forceComplete.InnerWidth;

            ImGui.SetCursorPos(cursor with {X = cursor.X + availableArea / 2.0f - textSize.X / 2.0f});
            ImGui.TextColored(Colors.Orange, Strings.Module.HuntMarks.NoUndo);

            ImGui.BeginDisabled(!keys);
            if (ImGui.Button(Strings.Module.HuntMarks.ForceComplete, new Vector2(forceComplete.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale)))
            {
                Settings.FauxHollowsCompleted = GetRequiredCompletionCount();
            }
            ImGui.EndDisabled();
        }

        private static int GetRequiredCompletionCount()
        {
            return Settings.IncludeRetelling.Value ? 2 : 1;
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = Service.PayloadManager.AddChatLink(ChatPayloads.OpenPartyFinder, OpenPartyFinder);

            Service.AddonManager.Get<WeeklyPuzzleAddon>().Show += OnShow;
        }
        
        public void Dispose()
        {
            Service.AddonManager.Get<WeeklyPuzzleAddon>().Show -= OnShow;
        }

        private void OnShow(object? sender, IntPtr e)
        {
            if (!Settings.Enabled.Value) return;

            Settings.FauxHollowsCompleted += 1;
            Service.ConfigurationManager.Save();
        }

        private void OpenPartyFinder(uint arg1, SeString arg2)
        {
            Service.ChatManager.SendCommandUnsafe("partyfinder");
        }

        public string GetStatusMessage() => $"{Strings.Module.UnrealTrial.TrialAvailable}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            Settings.FauxHollowsCompleted = 0;
        }

        public ModuleStatus GetModuleStatus() => Settings.FauxHollowsCompleted >= GetRequiredCompletionCount() ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private static int GetRequiredCompletionCount()
        {
            return Settings.IncludeRetelling.Value ? 2 : 1;
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

        public string GetShortTaskLabel() => Strings.Module.UnrealTrial.Label;

        public string GetLongTaskLabel() => Strings.Module.UnrealTrial.Label;
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