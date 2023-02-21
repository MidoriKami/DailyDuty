using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

internal class ConfigurationWindow : SelectionWindow, IDisposable
{

    public ConfigurationWindow() : base($"DailyDuty {Strings.Config_Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyMainWindow", 47.0f)
    {
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

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e) => WindowName = $"DailyDuty {Strings.Config_Label} - {e.CharacterData.Name}###DailyDutyMainWindow";

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
    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }

    private static void DrawNavigationButtons()
    {
        var itemSize = ImGuiHelpers.ScaledVector2(23.0f);
        var region = ImGui.GetContentRegionAvail();
        var padding = ImGui.GetStyle().ItemSpacing;

        var buttonsTotalSize = itemSize * 3 + padding * 2;
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 2.0f - buttonsTotalSize.X / 2.0f, Y = padding.Y});
        
        if (ImGuiComponents.IconButton("ConfigurationButton", FontAwesomeIcon.Cog)) KamiCommon.WindowManager.ToggleWindowOfType<OverlayConfigurationWindow>();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Open Overlay Configuration");
        ImGui.SameLine();
        
        if (ImGuiComponents.IconButton("StatusButton", FontAwesomeIcon.InfoCircle)) KamiCommon.WindowManager.ToggleWindowOfType<StatusWindow>();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Open Task Status");
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        
        if (ImGuiComponents.IconButton("KoFiButton", FontAwesomeIcon.Coffee)) KamiCommon.WindowManager.ToggleWindowOfType<AboutWindow>();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Support Me On Ko-Fi");
        
        ImGui.PopStyleColor(3);
    }
}