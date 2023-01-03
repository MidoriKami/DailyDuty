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

    public TimersStyleWindow(IModule owner) : base($"{Strings.Timers_EditTimerStyle} - {owner.Name.GetTranslatedString()}")
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
            .AddTitle(Strings.Timers_TimeOptions, out var innerWidth)
            .AddConfigCombo(TimerStyleExtensions.GetConfigurableStyles(), OwnerModule.GenericSettings.TimerSettings.TimerStyle, TimerStyleExtensions.GetLabel, width: innerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Timers_DisplayName)
            .AddConfigCheckbox(Strings.Timers_EnableCustomName, Settings.UseCustomName)
            .AddConfigString(Settings.CustomName, innerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Timers_TextOptions)
            .AddConfigCheckbox(Strings.Timers_HideLabel, Settings.HideLabel)
            .AddConfigCheckbox(Strings.Timers_HideTime, Settings.HideTime)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Timers_ColorOptions)
            .AddConfigColor(Strings.Common_Background, Strings.Common_Default, Settings.BackgroundColor, Colors.Black)
            .AddConfigColor(Strings.Common_Foreground, Strings.Common_Default, Settings.ForegroundColor, Colors.Purple)
            .AddConfigColor(Strings.Common_Text, Strings.Common_Default, Settings.TextColor, Colors.White)
            .AddConfigColor(Strings.Timers_Label, Strings.Common_Default, Settings.TimeColor, Colors.White)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Timers_SizeOptions, out var innerWidth2)
            .AddConfigCheckbox(Strings.Timers_StretchToFit, Settings.StretchToFit)
            .AddSliderInt(Strings.Timers_Size, Settings.Size, 10, 500, innerWidth2 / 2.0f)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Common_Reset, out var innerWidth3)
            .AddDisabledButton(Strings.Common_Reset, () => { 
                OwnerModule.GenericSettings.TimerSettings = new TimerSettings();
                Settings = OwnerModule.GenericSettings.TimerSettings;
                Service.ConfigurationManager.Save();
            }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.DisabledButton_Hover, innerWidth3)
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