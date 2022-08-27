using System;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class ConfigurationWindow : Window, IDisposable
{
    private readonly SelectionFrame selectionFrame;
    private readonly ConfigurationFrame configurationFrame; 

    public ConfigurationWindow() : base($"DailyDuty {Strings.Configuration.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyMainWindow")
    {
        Log.Verbose("Constructing ConfigurationWindow");

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400 * (16.0f / 9.0f), 400),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        var selectables = Service.ModuleManager.GetConfigurationSelectables();

        selectionFrame = new SelectionFrame(selectables, 0.35f, true);
        configurationFrame = new ConfigurationFrame();

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
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
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }

}