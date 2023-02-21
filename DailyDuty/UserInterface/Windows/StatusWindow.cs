using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

internal class StatusWindow : SelectionWindow, IDisposable
{
    public StatusWindow() : base($"DailyDuty {Strings.Status_Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyStatusWindow", 45.0f)
    {
        KamiCommon.CommandManager.AddCommand(new OpenWindowCommand<StatusWindow>("status"));
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 450),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        Service.ConfigurationManager.OnCharacterDataLoaded += UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"DailyDuty {Strings.Status_Label} - {e.CharacterData.Name}###DailyDutyStatusWindow";
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataLoaded -= UpdateWindowTitle;
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    protected override IEnumerable<ISelectable> GetSelectables()
    {
        if (Service.ConfigurationManager.CharacterConfiguration.HideDisabledModulesInSelectWindow)
        {
            return Service.ModuleManager.GetStatusSelectables()
                .OfType<StatusSelectable>()
                .Where(selectable => selectable.ParentModule.GenericSettings.Enabled);
        }

        return Service.ModuleManager.GetStatusSelectables();
    }

    protected override void DrawExtras()
    {
        DrawHideDisabledButton();
        PluginVersion.Instance.DrawVersionText();
    }

    private static void DrawHideDisabledButton()
    {
        var config = Service.ConfigurationManager.CharacterConfiguration;
        
        var region = ImGui.GetContentRegionAvail();

        var label = config.HideDisabledModulesInSelectWindow ? Strings.Config_ShowDisabled : Strings.Config_HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            config.HideDisabledModulesInSelectWindow = !config.HideDisabledModulesInSelectWindow;
            Service.ConfigurationManager.Save();
        }
    }
}