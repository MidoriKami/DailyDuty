using System;
using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class TimersStyleWindow : Window, IDisposable
{
    public TimerSettings Settings { get; }
    private IModule OwnerModule { get; }

    private readonly InfoBox timeDisplay = new();
    private readonly InfoBox colorOptions = new();
    private readonly InfoBox sizeOptions = new();

    public TimersStyleWindow(IModule owner, TimerSettings settings, string windowName) : base(windowName)
    {
        Settings = settings;
        OwnerModule = owner;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200 * (4.0f / 3.0f), 300),
            MaximumSize = new Vector2(9999, 9999)
        };

        IsOpen = true;
    }

    public void Dispose()
    {

    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
    }

    public override void Draw()
    {
        timeDisplay
            .AddTitle(Strings.UserInterface.Timers.TimeDisplay)
            .AddConfigCombo(TimerStyleExtensions.GetConfigurableStyles(), OwnerModule.GenericSettings.TimerSettings.TimerStyle,
                TimerStyleExtensions.GetLabel, width: 175.0f)
            .Draw();

        colorOptions
            .AddTitle(Strings.UserInterface.Timers.ColorOptions)
            .AddConfigColor(Strings.UserInterface.Timers.Background, Settings.BackgroundColor)
            .AddConfigColor(Strings.UserInterface.Timers.Foreground, Settings.ForegroundColor)
            .AddConfigColor(Strings.UserInterface.Timers.Text, Settings.TextColor)
            .AddConfigColor(Strings.UserInterface.Timers.Time, Settings.TimeColor)
            .Draw();

        sizeOptions
            .AddTitle(Strings.UserInterface.Timers.SizeOptions)
            .AddConfigCheckbox(Strings.UserInterface.Timers.StretchToFit, Settings.StretchToFit)
            .AddSliderInt(Strings.UserInterface.Timers.Size, Settings.Size, 10, 500, 125.0f)
            .Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
    }

    public override void OnClose()
    {
        Service.WindowManager.RemoveTimerStyleWindow(this);
    }
}