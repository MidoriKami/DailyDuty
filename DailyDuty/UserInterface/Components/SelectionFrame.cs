using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components;

internal class SelectionFrame : IDrawable
{
    public ISelectable? Selected { get; private set; }

    private IEnumerable<ISelectable> Selectables { get; }

    private float Weight { get; }

    private readonly string pluginVersion;

    public SelectionFrame(IEnumerable<ISelectable> selectables, float weight = 0.30f)
    {
        Selectables = new List<ISelectable>(selectables);
        Weight = weight;
        
        pluginVersion = GetVersionText();
    }

    public void Draw()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();

        if (ImGui.BeginChild("###SelectionFrame", new Vector2(regionAvailable.X * Weight, 0), false))
        {
            var frameBgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            ImGui.PushStyleColor(ImGuiCol.FrameBg, frameBgColor with { W = 0.05f });

            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
            if (ImGui.BeginListBox("", new Vector2(-1, -25)))
            {
                ImGui.PopStyleColor(1);

                foreach (var item in Selectables)
                {
                    ImGui.PushID(item.OwnerModuleName.ToString());

                    var headerHoveredColor = ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderHovered];
                    var textSelectedColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerHoveredColor with { W = 0.1f });
                    ImGui.PushStyleColor(ImGuiCol.Header, textSelectedColor with { W = 0.1f });

                    if (ImGui.Selectable("", Selected == item))
                    {
                        Selected = Selected == item ? null : item;
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();

                    ImGui.SameLine(3.0f);

                    item.DrawLabel();

                    ImGui.Spacing();

                    ImGui.PopID();
                }

                ImGui.EndListBox();
            }
            ImGui.PopStyleVar();

            DrawVersionText();
        }

        ImGui.EndChild();

        ImGui.SameLine();
    }

    private string GetVersionText()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

        var versionString = assemblyInformation[1].Replace('=', ' ');

        var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        return $"{versionString} - {commitInfo}";
    }

    private void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(pluginVersion) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(Colors.Grey, pluginVersion);
    }
}