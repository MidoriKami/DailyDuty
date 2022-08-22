using DailyDuty.Configuration.OverlaySettings;
using DailyDuty.UserInterface.Components.InfoBox;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using DailyDuty.System.Localization;

namespace DailyDuty.UserInterface.Windows;

internal class TimersConfigurationWindow : Window, IDisposable
{
    public static TimersOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TimersOverlay;

    private readonly InfoBox mainOptionsInfoBox = new();
    private readonly InfoBox windowHidingOptionsInfoBox = new();
    private readonly InfoBox timersSelectionInfoBox = new();

    public TimersConfigurationWindow() : base("DailyDuty Timers Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public void Dispose()
    {

    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
    }

    public override void Draw()
    {
        mainOptionsInfoBox
            .AddTitle(Strings.UserInterface.Timers.MainOptions)
            .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
            .Draw();

        var enabledModules = Service.ModuleManager.GetTimerComponents()
            .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

        timersSelectionInfoBox
            .AddTitle(Strings.UserInterface.Timers.Label)
            .AddTimerComponents(enabledModules)
            .Draw();
        
        windowHidingOptionsInfoBox
            .AddTitle(Strings.UserInterface.Timers.WindowOptions)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideWindowInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.UserInterface.Timers.LockWindow, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.UserInterface.Timers.AutoResize, Settings.AutoResize)
            .AddDragFloat(Strings.UserInterface.Timers.Opacity, Settings.Opacity, 0.0f, 1.0f, 200.0f)
            .Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
    }

    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}