using System;
using System.Globalization;
using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleResetView
{
    public static void Draw(ModuleDataBase data)
    {
        ImGui.Text(Strings.ModuleReset);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        if (data.NextReset != DateTime.MaxValue)
        {
            if (ImGui.BeginTable("##ResetTable", 2, ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableNextColumn();
                ImGui.Text(Strings.NextReset);
            
                ImGui.TableNextColumn();
                ImGui.Text(data.NextReset.ToLocalTime().ToString(CultureInfo.CurrentCulture));

                ImGui.TableNextColumn();
                ImGui.Text(Strings.RemainingTime);
            
                ImGui.TableNextColumn();
                var timeRemaining = data.NextReset - DateTime.UtcNow;
                ImGui.Text($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
            
                ImGui.EndTable();
            }
        }
        else
        {
            ImGui.Text(Strings.AwaitingUserAction);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}