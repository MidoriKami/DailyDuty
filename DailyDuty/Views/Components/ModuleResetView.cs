using System;
using System.Globalization;
using DailyDuty.Abstracts;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleResetView
{
    public static void Draw(ModuleDataBase data)
    {
        ImGui.Text("Module Reset");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        if (ImGui.BeginTable("##ResetTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Next Reset");
            
            ImGui.TableNextColumn();
            ImGui.Text(data.NextReset.ToLocalTime().ToString(CultureInfo.CurrentCulture));

            ImGui.TableNextColumn();
            ImGui.Text("Remaining Time");
            
            ImGui.TableNextColumn();
            var timeRemaining = data.NextReset - DateTime.UtcNow;
            ImGui.Text($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
            
            ImGui.EndTable();
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}