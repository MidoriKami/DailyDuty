using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

internal class ConfigurationWindow : SelectionWindow, IDisposable
{

    public ConfigurationWindow() : base($"DailyDuty {Strings.Configuration.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyMainWindow", 0.35f, 40.0f)
    {
        KamiCommon.CommandManager.AddCommand(new ConfigurationWindowCommands<ConfigurationWindow>());
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 450),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        Service.ConfigurationManager.OnCharacterDataLoaded += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataLoaded -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e) => WindowName = $"DailyDuty {Strings.Configuration.Label} - {e.CharacterData.Name}###DailyDutyMainWindow";

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    protected override IEnumerable<ISelectable> GetSelectables() => Service.ModuleManager.GetConfigurationSelectables();

    protected override void DrawExtras()
    {
        DrawNavigationButtons();
        PluginVersion.Instance.DrawVersionText();
    }

    protected override void DrawSpecial()
    {
        AboutWindow.DrawInfoButton();
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }

    private static void DrawNavigationButtons()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(2.0f, 0.0f) * ImGuiHelpers.GlobalScale);

        var contentRegion = ImGui.GetContentRegionAvail();
        var buttonWidth = contentRegion.X / 3.0f - 2.0f * ImGuiHelpers.GlobalScale;

        if (ImGui.Button(Strings.UserInterface.Todo.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = KamiCommon.WindowManager.GetWindowOfType<TodoConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.SameLine();

        if (ImGui.Button(Strings.UserInterface.Timers.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = KamiCommon.WindowManager.GetWindowOfType<TimersConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.SameLine();

        if (ImGui.Button(Strings.Status.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = KamiCommon.WindowManager.GetWindowOfType<StatusWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.PopStyleVar();
    }
}