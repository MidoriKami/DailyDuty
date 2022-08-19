using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components.InfoBox;

internal class InfoBoxTable
{
    private readonly InfoBox owner;
    private readonly float weight;

    private readonly List<Tuple<Action?, Action?>> tableRows = new();

    public InfoBoxTable(InfoBox owner, float weight = 0.5f)
    {
        this.owner = owner;
        this.weight = weight;
    }

    public InfoBoxTable AddRow(string label, string contents, Vector4? firstColor = null, Vector4? secondColor = null)
    {
        tableRows.Add(new Tuple<Action?, Action?>(
                Actions.GetStringAction(label, firstColor), 
                Actions.GetStringAction(contents, secondColor)
                ));

        return this;
    }

    public InfoBoxTable AddEnumerable(IEnumerable<IInfoBoxTableRow> list)
    {
        foreach (var row in list)
        {
            tableRows.Add(row.GetInfoBoxTableRow());
        }

        return this;
    }

    public InfoBoxTable AddActions(Action? firstAction, Action? secondAction)
    {
        tableRows.Add(new Tuple<Action?, Action?>(firstAction, secondAction));

        return this;
    }

    public InfoBox EndTable()
    {
        owner.AddAction(() =>
        {
            if (ImGui.BeginTable($"", 2, ImGuiTableFlags.None, new Vector2(owner.InnerWidth, 0)))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (weight) );
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (1 - weight) );

                foreach (var row in tableRows)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    ImGui.PushTextWrapPos(GetWrapPosition());
                    row.Item1?.Invoke();
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos(GetWrapPosition());
                    row.Item2?.Invoke();
                    ImGui.PopTextWrapPos();
                }

                ImGui.EndTable();
            }
        });

        return owner;
    }

    private float GetWrapPosition()
    {
        var region = ImGui.GetContentRegionAvail();

        var cursor = ImGui.GetCursorPos();

        var wrapPosition = cursor.X + region.X;

        return wrapPosition;
    }
}