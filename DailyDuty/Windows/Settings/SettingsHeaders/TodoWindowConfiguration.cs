using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects.WindowSettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.SettingsHeaders;

internal class TodoWindowConfiguration : ICollapsibleHeader
{
    private TodoWindowSettings Settings => Service.Configuration.TodoWindowSettings;

    public string HeaderText => "Todo Window Configuration";

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

        ShowHideWindow();

        DisableEnableClickThrough();

        TaskSelection();

        HideInDuty();

        HideWhenComplete();

        OpacitySlider();

        EditColors();

        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
    }
    
    public void Dispose()
    {
            
    }

    private void OpacitySlider()
    {
        ImGui.PushItemWidth(150);
        ImGui.DragFloat($"Opacity##{HeaderText}", ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
        ImGui.PopItemWidth();
    }

    private void HideWhenComplete()
    {
        ImGui.Checkbox("Hide when Complete", ref Settings.HideWhenTasksComplete);
        ImGuiComponents.HelpMarker("Hides the todo window when the selected task list is complete.");
    }

    private void HideInDuty()
    {
        ImGui.Checkbox("Hide when Bound By Duty", ref Settings.HideInDuty);
    }

    private void TaskSelection()
    {
        Draw.NotificationField("Daily Tasks", HeaderText, ref Settings.ShowDaily, "Show/Hide Daily Tasks category");

        Draw.NotificationField("Weekly Tasks", HeaderText, ref Settings.ShowWeekly, "Show/Hide Weekly Tasks category");
    }

    private void DisableEnableClickThrough()
    {
        Draw.NotificationField("Enable Click-through", HeaderText, ref Settings.ClickThrough, "Enables/Disables the ability to move the Todo Window");
    }

    private void ShowHideWindow()
    {
        Draw.NotificationField("Show Todo Window", HeaderText, ref Settings.Open, "Shows/Hides the Todo Window");
    }

    private void EditColors()
    {
        ImGui.ColorEdit4("Header Color", ref Settings.HeaderColor, ImGuiColorEditFlags.NoInputs);
        ImGui.ColorEdit4("Completed Task Color", ref Settings.CompleteColor, ImGuiColorEditFlags.NoInputs);
        ImGui.ColorEdit4("Incomplete Task Color", ref Settings.IncompleteColor, ImGuiColorEditFlags.NoInputs);
    }
}