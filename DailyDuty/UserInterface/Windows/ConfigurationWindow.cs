using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.Tabs;
using DailyDuty.Utilities;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

public class ConfigurationWindow : TabbedSelectionWindow, IDisposable
{
    private readonly List<ISelectionWindowTab> tabs = new()
    {
        new ModuleConfigurationTabItem(),
        new ModuleStatusTabItem(),
        new OverlayConfigurationTabItem(),
        new TimerStyleTabItem(),
    };
    
    public ConfigurationWindow() : base($"DailyDuty {Strings.Config_Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyMainWindow", 55.0f)
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

    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;
    
    protected override void DrawWindowExtras()
    {
        base.DrawWindowExtras();
        PluginVersion.Instance.DrawVersionText();
    }
    
    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }
}