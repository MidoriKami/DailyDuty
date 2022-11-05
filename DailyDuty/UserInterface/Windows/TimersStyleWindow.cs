using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class TimersStyleWindow : Window
{
    public TimerSettings Settings { get; }
    private IModule OwnerModule { get; }

    public TimersStyleWindow(IModule owner) : base($"{Strings.UserInterface.Timers.EditTimerTitle} - {owner.Name.GetTranslatedString()}")
    {
        OwnerModule = owner;
        Settings = owner.GenericSettings.TimerSettings;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200 * (4.0f / 3.0f), 300),
            MaximumSize = new Vector2(9999, 9999)
        };

        IsOpen = true;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.TimeDisplay)
            .AddConfigCombo(TimerStyleExtensions.GetConfigurableStyles(), OwnerModule.GenericSettings.TimerSettings.TimerStyle, TimerStyleExtensions.GetLabel, width: 175.0f)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.Name)
            .AddConfigCheckbox(Strings.UserInterface.Timers.EnableCustomName, Settings.UseCustomName)
            .AddConfigString(Settings.CustomName)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.TextOptions)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideLabel, Settings.HideLabel)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideTime, Settings.HideTime)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.ColorOptions)
            .AddConfigColor(Strings.UserInterface.Timers.Background, Settings.BackgroundColor)
            .AddConfigColor(Strings.UserInterface.Timers.Foreground, Settings.ForegroundColor)
            .AddConfigColor(Strings.UserInterface.Timers.Text, Settings.TextColor)
            .AddConfigColor(Strings.UserInterface.Timers.Time, Settings.TimeColor)
            .Draw();

        InfoBox.Instance
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
        Service.WindowManager.RemoveWindow(this);
    }
}