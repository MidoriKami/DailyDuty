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
            .Select(module => new TimerStyleSelectable(module.ParentModule));
    }
}