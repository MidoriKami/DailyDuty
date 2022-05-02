using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Timers
{
    internal class TimersTabItem : ITabItem
    {
        private static MainWindowSettings Settings => Service.SystemConfiguration.Windows.MainWindow;
        
        private readonly List<CountdownTimer> timers = Service.TimerManager.Timers;

        public ModuleType ModuleType => ModuleType.Timers;

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Timers.TimersLabel);
        }

        public void DrawConfigurationPane()
        {

            ImGuiHelpers.ScaledDummy(20.0f);
            ImGui.ColorEdit4(Strings.Timers.TimersBarColorLabel, ref Settings.TimerProgressBarColor);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Checkbox(Strings.Timers.TimersHideSecondsLabel, ref Settings.HideSeconds);

            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(5.0f);

            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, Settings.TimerProgressBarColor);

            foreach (var timer in timers.OrderByDescending(t => t.Label))
            {
                timer.Draw();

                ImGuiHelpers.ScaledDummy(10.0f);
            }

            ImGui.PopStyleColor();
        }
    }
}
