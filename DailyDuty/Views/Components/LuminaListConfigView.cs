using System;
using System.Collections.Generic;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel;

namespace DailyDuty.Views.Components;

public static class LuminaListConfigView
{
    public static void Draw<T>(BaseModule module, List<LuminaTaskConfig> tasks, Func<T, string> getLuminaString) where T : ExcelRow
    {
        ImGui.Text("Toggleable Tasks");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        foreach (var taskConfig in tasks)
        {
            var luminaObject = LuminaCache<T>.Instance.GetRow(taskConfig.RowId);
            if (luminaObject is null) continue;

            var enabled = taskConfig.Enabled;
            if (ImGui.Checkbox(getLuminaString(luminaObject), ref enabled))
            {
                taskConfig.Enabled = enabled;
                module.SaveConfig();
            }
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}