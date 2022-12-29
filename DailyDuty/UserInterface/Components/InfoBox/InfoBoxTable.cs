using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using ImGuiNET;
using KamiLib.Utilities;

namespace DailyDuty.UserInterface.Components.InfoBox;

public class InfoBoxTable
{
    private readonly InfoBox owner;
    private readonly float weight;

    private readonly List<InfoBoxTableRow> rows = new();
    private string emptyListString = string.Empty;

    public InfoBoxTable(InfoBox owner, float weight = 0.5f)
    {
        this.owner = owner;
        this.weight = weight;
    }

    public InfoBoxTableRow BeginRow()
    {
        return new InfoBoxTableRow(this);
    }

    public InfoBoxTable AddRow(InfoBoxTableRow row)
    {
        rows.Add(row);

        return this;
    }

    public InfoBox EndTable()
    {
        owner.AddAction(() =>
        {
            if (rows.Count == 0)
            {
                if (emptyListString != string.Empty)
                {
                    ImGui.TextColored(Colors.Orange, emptyListString);
                }
            }
            else
            {
                if (ImGui.BeginTable($"", 2, ImGuiTableFlags.None, new Vector2(owner.InnerWidth, 0)))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (weight));
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (1 - weight));

                    foreach (var row in rows)
                    {
                        ImGui.TableNextColumn();

                        ImGui.PushTextWrapPos(GetWrapPosition());
                        row.FirstColumn?.Invoke();
                        ImGui.PopTextWrapPos();

                        ImGui.TableNextColumn();
                        ImGui.PushTextWrapPos(GetWrapPosition());
                        row.SecondColumn?.Invoke();
                        ImGui.PopTextWrapPos();
                    }

                    ImGui.EndTable();
                }

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

    public InfoBoxTable AddRows(IEnumerable<IInfoBoxTableConfigurationRow> configurableRows, string? emptyEnumerableString = null)
    {
        if(emptyEnumerableString is not null)
        {
            emptyListString = emptyEnumerableString;
        }

        foreach (var row in configurableRows)
        {
            row.GetConfigurationRow(this);
        }

        return this;
    }

    public InfoBoxTable AddRows(IEnumerable<IInfoBoxTableDataRow> dataRows, string? emptyEnumerableString = null)
    {
        if(emptyEnumerableString is not null)
        {
            emptyListString = emptyEnumerableString;
        }

        foreach (var row in dataRows)
        {
            row.GetDataRow(this);
        }

        return this;
    }
}