using System.Numerics;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.InfoBoxSystem;
using KamiLib.Utilities;

namespace DailyDuty.UserInterface.Windows;

internal class TimersStyleWindow : Window
{
    private TimerSettings Settings { get; set; }
    private IModule OwnerModule { get; }

    public TimersStyleWindow(IModule owner) : base($"{Strings.UserInterface.Timers.EditTimerTitle} - {owner.Name.GetTranslatedString()}")
    {
        OwnerModule = owner;
        Settings = owner.GenericSettings.TimerSettings;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(275, 300),
            MaximumSize = new Vector2(9999, 9999)
        };

        IsOpen = true;
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
        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.TimeDisplay, out var innerWidth)
            .AddConfigCombo(TimerStyleExtensions.GetConfigurableStyles(), OwnerModule.GenericSettings.TimerSettings.TimerStyle, TimerStyleExtensions.GetLabel, width: innerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.Name)
            .AddConfigCheckbox(Strings.UserInterface.Timers.EnableCustomName, Settings.UseCustomName)
            .AddConfigString(Settings.CustomName, innerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.TextOptions)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideLabel, Settings.HideLabel)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideTime, Settings.HideTime)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.ColorOptions)
            .AddConfigColor(Strings.UserInterface.Timers.Background, Strings.Common.Default, Settings.BackgroundColor, Colors.Black)
            .AddConfigColor(Strings.UserInterface.Timers.Foreground, Strings.Common.Default, Settings.ForegroundColor, Colors.Purple)
            .AddConfigColor(Strings.UserInterface.Timers.Text, Strings.Common.Default, Settings.TextColor, Colors.White)
            .AddConfigColor(Strings.UserInterface.Timers.Time, Strings.Common.Default, Settings.TimeColor, Colors.White)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.SizeOptions, out var innerWidth2)
            .AddConfigCheckbox(Strings.UserInterface.Timers.StretchToFit, Settings.StretchToFit)
            .AddSliderInt(Strings.UserInterface.Timers.Size, Settings.Size, 10, 500, innerWidth2 / 2.0f)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.Reset, out var innerWidth3)
            .AddDisabledButton(Strings.UserInterface.Timers.Reset, () => { 
                OwnerModule.GenericSettings.TimerSettings = new TimerSettings();
                Settings = OwnerModule.GenericSettings.TimerSettings;
                Service.ConfigurationManager.Save();
            }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.Module.Raids.RegenerateTooltip, innerWidth3)
            .Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
    }

    public override void OnClose()
    {
        KamiCommon.WindowManager.RemoveWindow(this);
    }
}