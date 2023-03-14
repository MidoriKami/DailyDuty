using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel;

namespace DailyDuty.Views.Components;

public static class LuminaListDataView
{
    public static void Draw<T>(List<LuminaTaskData> tasks, List<LuminaTaskConfig> taskConfig, Func<T, string> getLuminaString) where T : ExcelRow
    {
        ImGui.Text("Toggleable Task Status");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        if (taskConfig.Any(config => config.Enabled))
        {
            if (ImGui.BeginTable("##TaskDataTable", 2, ImGuiTableFlags.SizingStretchSame))
            {
                foreach (var taskData in tasks)
                {
                    var luminaObject = LuminaCache<T>.Instance.GetRow(taskData.RowId);
                    if (luminaObject is null) continue;

                    var enabledState = taskConfig.First(task => task.RowId == taskData.RowId);
                    if (enabledState.Enabled)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text(getLuminaString(luminaObject));

                        ImGui.TableNextColumn();
                        var taskComplete = taskData.Complete;
                        ImGui.TextColored(taskComplete ? KnownColor.Green.AsVector4() : KnownColor.Red.AsVector4()  ,taskComplete ? "Complete" : "Incomplete");
                    }
                }
                
                ImGui.EndTable();
            }
        }
        else
        {
            var region = ImGui.GetContentRegionAvail();
            const string infoMessage = "No Tasks are Enabled";
            var messageSize = ImGui.CalcTextSize(infoMessage);
            var regionStart = region.X / 2.0f + 7.5f * ImGuiHelpers.GlobalScale;
            
            ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = regionStart - messageSize.X / 2.0f});
            ImGui.TextColored(KnownColor.Orange.AsVector4(), infoMessage);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}