using System.Collections.Generic;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Graphical.TabItems
{
    internal class LogDebugTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.LogDebug;

        private readonly InfoBox logOperations = new()
        {
            Label = "Log Operations",
            ContentsAction = () =>
            {
                if (ImGui.Button("Purge Log"))
                {
                    Service.LogManager.Log.Messages = new Dictionary<ModuleType, List<LogMessage>>();
                    Service.LogManager.Save();
                }
            }
        };

        private readonly InfoBox logStatus = new()
        {
            Label = "Log Status",
            ContentsAction = () =>
            {
                if (Service.LogManager.Log.Messages.Keys.Count == 0)
                {
                    ImGui.Text("No Logs Found");
                }
                else
                {
                    foreach (var key in Service.LogManager.Log.Messages.Keys)
                    {
                        ImGui.Text($"[{key}]: {Service.LogManager.Log.Messages[key].Count}");
                    }
                }
            }
        };

        public void DrawTabItem()
        {
            ImGui.Text("Log Debug Tools");
        }

        public void DrawConfigurationPane()
        {
            if(ImGui.BeginChild("###DebugChild"))
            {
                ImGuiHelpers.ScaledDummy(10.0f);
                logOperations.DrawCentered();

                ImGuiHelpers.ScaledDummy(30.0f);
                logStatus.DrawCentered();
            
                ImGuiHelpers.ScaledDummy(30.0f);
            }
            ImGui.EndChild();
        }
    }
}
