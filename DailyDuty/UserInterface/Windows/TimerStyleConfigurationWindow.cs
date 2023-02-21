using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.UserInterface.Components;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

public class TimerStyleConfigurationWindow : SelectionWindow
{
    public TimerStyleConfigurationWindow() : base("Timer Style Configuration Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 450),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }
    
    protected override IEnumerable<ISelectable> GetSelectables()
    {
        return Service.ModuleManager.GetTimerComponents()
            .Where(timer => timer.ParentModule.GenericSettings.TimerTaskEnabled)
            .Where(module => module.ParentModule.GenericSettings.Enabled)
            .Select(module => new TimerStyleSelectable(module.ParentModule))
            .DefaultIfEmpty<ISelectable>(new NoTimersEnabledSelectable());
    }
}

public class NoTimersEnabledSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    public string ID => "NoTimersEnabled";
    
    public void DrawLabel()
    {
        ImGui.Text("No Timers Enabled");
    }

    public void Draw()
    {
        var region = ImGui.GetContentRegionAvail();

        const string textLine = "No Timers Enabled";
        const string secondLine = "Enable in Overlay Configuration";

        var textSize = ImGui.CalcTextSize(textLine);
        var textSize2 = ImGui.CalcTextSize(secondLine);

        ImGui.SetCursorPos(new Vector2(region.X / 2.0f - textSize.X / 2.0f, region.Y / 2.0f - textSize.Y / 2.0f));
        ImGui.TextUnformatted(textLine);
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 2.0f - textSize2.X / 2.0f });
        ImGui.TextUnformatted(secondLine);
    }
}