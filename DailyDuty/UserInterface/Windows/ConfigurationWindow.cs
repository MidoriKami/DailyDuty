using System;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.CommandSystem;

namespace DailyDuty.UserInterface.Windows;

internal class ConfigurationWindow : Window, IDisposable
{
    private readonly SelectionFrame selectionFrame;
    private readonly ConfigurationFrame configurationFrame; 

    public ConfigurationWindow() : base($"DailyDuty {Strings.Configuration.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyMainWindow")
    {
        KamiCommon.CommandManager.AddCommand(new ConfigurationWindowCommands<ConfigurationWindow>());
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(450 * (16.0f / 9.0f), 450),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        var selectables = Service.ModuleManager.GetConfigurationSelectables();

        selectionFrame = new SelectionFrame(selectables, 0.35f, new NavigationButtons());
        configurationFrame = new ConfigurationFrame();

        Service.ConfigurationManager.OnCharacterDataLoaded += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataLoaded -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"DailyDuty {Strings.Configuration.Label} - {e.CharacterData.Name}###DailyDutyMainWindow";
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {
        selectionFrame.Draw();

        configurationFrame.Draw(selectionFrame.Selected);
        
        AboutWindow.DrawInfoButton();
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }
}