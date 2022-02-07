using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Components.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Settings.SettingsHeaders;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Settings;

internal class SettingsWindow : Window, IDisposable
{

    private readonly AllCountdownTimers comboCountdown = new();
    private readonly SaveAndCloseButtons saveAndCloseButtons;

    public SettingsWindow() : base("DailyDuty Settings")
    {
        saveAndCloseButtons = new(this);

        Service.WindowSystem.AddWindow(this);

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new(265, 250),
            MaximumSize = new(9909,9909)
        };


        //Flags |= ImGuiWindowFlags.NoResize;
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose()
    {
        foreach (var tab in tabs)
        {
            tab.Dispose();
        }

        Service.WindowSystem.RemoveWindow(this);
    }

    private readonly List<ITabItem> tabs = new()
    {
        new OverviewTabItem(),
        new DailyTabItem(),
        new WeeklyTabItem(),
        new ConfigurationTabItem()
    };

    public override void Draw()
    {
        if (!IsOpen) return;

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(10, 5));

        comboCountdown.Draw();

        ImGui.Spacing();

        DrawTabs();

        saveAndCloseButtons.Draw();

        ImGui.PopStyleVar();
    }
        
    private void DrawTabs()
    {
        if (ImGui.BeginTabBar("DailyDutyTabBar", ImGuiTabBarFlags.NoTooltip))
        {
            foreach (var tab in tabs)
            {
                if (!ImGui.BeginTabItem(tab.TabName))
                    continue;

                // Stolen from https://git.annaclemens.io/ascclemens/ChatTwo/src/branch/main/ChatTwo/Ui/Settings.cs#L69
                var height = ImGui.GetContentRegionAvail().Y - 30 * ImGuiHelpers.GlobalScale;

                if (ImGui.BeginChild("DailyDutySettings", new Vector2(0, height), true)) 
                {
                    tab.Draw();
                    ImGui.EndChild();
                }

                ImGui.EndTabItem();
            }
        }
    }

    public override void OnClose()
    {
        ConfigurationTabItem.EditModeEnabled = false;

        base.OnClose();
    }
}