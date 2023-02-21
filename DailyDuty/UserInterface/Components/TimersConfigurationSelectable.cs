using System.Linq;
using DailyDuty.Commands;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using ImGuiNET;
using KamiLib;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components;

public class TimersOverlaySettings
{
    public Setting<bool> Enabled = new(false);
    public Setting<bool> HideWhileInDuty = new(true);
    public Setting<bool> LockWindowPosition = new(false);
    public Setting<bool> AutoResize = new(true);
    public Setting<bool> HideCompleted = new(false);
    public Setting<float> Opacity = new(1.0f);
    public Setting<TimersOrdering> Ordering = new(TimersOrdering.Alphabetical);
}

public class TimersConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;

    public string ID => "Timers Overlay";
    
    private static TimersOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TimersOverlay;

    public TimersConfigurationSelectable()
    {
        KamiCommon.CommandManager.AddCommand(new TimersCommands());
    }
    
    public void DrawLabel()
    {
        ImGui.Text(ID);
        DrawModuleStatus();
    }
    
    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();

        var text = Settings.Enabled ? Strings.Common_Enabled : Strings.Common_Disabled;
        var color = Settings.Enabled ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Common_MainOptions)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
            .AddConfigCheckbox(Strings.Common_HideCompletedTasks, Settings.HideCompleted)
            .Draw();

        var enabledModules = Service.ModuleManager.GetTimerComponents()
            .Where(module => module.ParentModule.GenericSettings.Enabled);

        InfoBox.Instance
            .AddTitle(Strings.Timers_Label)
            .BeginTable(0.65f)
            .AddConfigurationRows(enabledModules.Select(task => task.GetTimersConfigurationRow()), Strings.Timers_NothingEnabled)
            .EndTable()
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Timers_Ordering)
            .AddConfigRadio(TimersOrdering.Alphabetical.GetTranslatedString(), Settings.Ordering, TimersOrdering.Alphabetical)
            .AddConfigRadio(TimersOrdering.AlphabeticalDescending.GetTranslatedString(), Settings.Ordering, TimersOrdering.AlphabeticalDescending)
            .AddConfigRadio(TimersOrdering.TimeRemaining.GetTranslatedString(), Settings.Ordering, TimersOrdering.TimeRemaining)
            .AddConfigRadio(TimersOrdering.TimeRemainingDescending.GetTranslatedString(), Settings.Ordering, TimersOrdering.TimeRemainingDescending)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Common_WindowOptions, out var innerWidth)
            .AddConfigCheckbox(Strings.Common_HideInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.Common_LockWindowPosition, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.Common_AutoResize, Settings.AutoResize)
            .AddDragFloat(Strings.Common_Opacity, Settings.Opacity, 0.0f, 1.0f, innerWidth / 2.0f)
            .Draw();
    }
}