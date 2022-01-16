using System.Linq.Expressions;
using System.Numerics;
using DailyDuty.System;
using DailyDuty.System.Modules;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class ToDoTab : TabCategory
    {
        public ToDoTab()
        {
            CategoryName = "Outstanding Tasks";
            TabName = "Todo List";

            FrameID = (uint) DisplayManager.Tab.ToDo;

            Modules = new()
            {
                new ToDoModule()
            };
        }

        public class ToDoModule : DisplayModule
        {
            public override void Draw()
            {
                ImGui.Spacing();

                DrawContents();

                ImGui.Spacing();
            }

            protected override void DrawContents()
            {
                DrawDailyTasks();

                DrawWeeklyTasks();
            }

            private void DrawDailyTasks()
            {
                DrawSeparatedText("Daily Tasks");
                
                ImGui.Indent(15);

                bool anyTasks = false;

                if (TreasureMapModule.IsTreasureMapAvailable() == true)
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), "Daily Treasure Map");
                    ImGui.Spacing();
                    anyTasks = true;
                }

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15);
            }

            private static void DrawWeeklyTasks()
            {
                DrawSeparatedText("Weekly Tasks");
                
                ImGui.Indent(15);

                bool anyTasks = false;

                if (WondrousTailsModule.IsWondrousTailsBookComplete() == false)
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), "Weekly Wondrous Tails");
                    ImGui.Spacing();
                    anyTasks = true;
                }

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15);
            }

            private static void DrawSeparatedText(string text)
            {
                ImGui.Text(text);
                ImGui.Separator();
                ImGui.Spacing();
            }

            public override void Dispose()
            {
                
            }
        }

    }
}
