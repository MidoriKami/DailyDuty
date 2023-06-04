using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Utilities;

namespace DailyDuty.Views.Components;

public static class ModuleStatusView
{
    public static void Draw(BaseModule module)
    {
        ImGui.Text(Strings.ModuleStatus);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        if (ImGui.BeginTable("##StatusTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.Text(Strings.CurrentStatus);

            ImGui.TableNextColumn();
            ImGui.TextColored(module.ModuleStatus.GetColor(), module.ModuleStatus.GetLabel());
            
            ImGui.EndTable();
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}