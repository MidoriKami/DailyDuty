using DailyDuty.Configuration.OverlaySettings;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;

namespace DailyDuty.UserInterface.Windows;

internal class TimersConfigurationWindow : Window
{
    public static TimersOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TimersOverlay;

    public TimersConfigurationWindow() : base("DailyDuty Timers Configuration", ImGuiWindowFlags.AlwaysVerticalScrollbar)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999, 9999)
        };
    }
    
    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.MainOptions)
            .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideCompleted, Settings.HideCompleted)
            .Draw();

        var enabledModules = Service.ModuleManager.GetTimerComponents()
            .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.Label)
            .BeginTable(0.65f)
            .AddRows(enabledModules, Strings.UserInterface.Todo.NoTasksEnabled)
            .EndTable()
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Timers.WindowOptions)
            .AddConfigCheckbox(Strings.UserInterface.Timers.HideWindowInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.UserInterface.Timers.LockWindow, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.UserInterface.Timers.AutoResize, Settings.AutoResize)
            .AddDragFloat(Strings.UserInterface.Timers.Opacity, Settings.Opacity, 0.0f, 1.0f, 200.0f)
            .Draw();
    }

    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}