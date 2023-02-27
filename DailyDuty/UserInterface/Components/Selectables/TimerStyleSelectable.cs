using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using ImGuiNET;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components;

public class TimerStyleSelectable : ISelectable, IDrawable
{
    private readonly IModule module;
    private TimerSettings Settings { get; set; }
    public IDrawable Contents => this;

    public string ID => module.Name.GetTranslatedString();

    public TimerStyleSelectable(IModule module)
    {
        this.module = module;
        Settings = this.module.GenericSettings.TimerSettings;
    }
    
    public void DrawLabel()
    {
        ImGui.Text(ID);
        DrawModuleStatus();
    }
    
    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();

        var enabled = module.GenericSettings.TimerTaskEnabled && module.GenericSettings.Enabled;
        
        var text = enabled ? Strings.Common_Enabled : Strings.Common_Disabled;
        var color = enabled ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Common_MainOptions)
            .AddConfigCheckbox(Strings.Common_Enabled, module.GenericSettings.TimerTaskEnabled)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Timers_TimeOptions, out var innerWidth)
            .AddConfigCombo(TimerStyleExtensions.GetConfigurableStyles(), module.GenericSettings.TimerSettings.TimerStyle, TimerStyleExtensions.GetLabel, width: innerWidth)
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
            .AddConfigColor(Strings.Common_Time, Strings.Common_Default, Settings.TimeColor, Colors.White)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Timers_SizeOptions, out var innerWidth2)
            .AddConfigCheckbox(Strings.Timers_StretchToFit, Settings.StretchToFit)
            .AddSliderInt(Strings.Timers_Size, Settings.Size, 10, 500, innerWidth2 / 2.0f)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Common_Reset, out var innerWidth3)
            .AddDisabledButton(Strings.Common_Reset, () => { 
                module.GenericSettings.TimerSettings = new TimerSettings();
                Settings = module.GenericSettings.TimerSettings;
                Service.ConfigurationManager.Save();
            }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.DisabledButton_Hover, innerWidth3)
            .Draw();
    }
}