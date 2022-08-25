using System;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class StatusWindow : Window, IDisposable
{
    private readonly SelectionFrame selectionFrame;
    private readonly ConfigurationFrame configurationFrame; 

    public StatusWindow() : base($"DailyDuty {Strings.Status.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}###DailyDutyStatusWindow")
    {
        Log.Verbose("Constructing StatusWindow");

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400 * (16.0f / 9.0f), 400),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        var selectables = Service.ModuleManager.GetStatusSelectables();

        selectionFrame = new SelectionFrame(selectables, 0.35f);
        configurationFrame = new ConfigurationFrame();

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"DailyDuty {Strings.Status.Label} - {e.CharacterData.Name}###DailyDutyStatusWindow";
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
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
}