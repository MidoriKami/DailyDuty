using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.System;
using DailyDuty.System.Utilities;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.Windows
{
    public class ToDoWindow : Window, IDisposable
    {
        public ToDoWindowSettings Settings => Service.Configuration.ToDoWindowSettings;

        private ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                ImGuiWindowFlags.NoTitleBar |
                                                ImGuiWindowFlags.NoScrollbar |
                                                ImGuiWindowFlags.NoCollapse;

        private ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                     ImGuiWindowFlags.NoDecoration |
                                                     ImGuiWindowFlags.NoInputs;

        private readonly ToDoTab.ToDoModule toDoTab = new();

        public ToDoWindow() : base("DailyDuty ToDo Window")
        {
            Service.Framework.Update += FrameWorkUpdate;

            IsOpen = Settings.Open;
        }

        private void FrameWorkUpdate(Framework framework)
        {
            IsOpen = Settings.Open;

            if (Settings.HideInDuty == true)
            {
                if (ConditionManager.IsBoundByDuty() == true)
                {
                    IsOpen = false;
                }
            }

            Flags = Settings.ClickThrough ? ClickThroughFlags : DefaultFlags;
        }

        public override void Draw()
        {
            if(Settings.ShowDaily)
                toDoTab.DrawDailyTasks();

            if (Settings.ShowWeekly)
                toDoTab.DrawWeeklyTasks();
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameWorkUpdate;

            Settings.Open = IsOpen;
            Service.Configuration.Save();
            toDoTab.Dispose();
        }
    }
}
