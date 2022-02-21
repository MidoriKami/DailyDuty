using System;
using ImGuiNET;

namespace DailyDuty.Interfaces;

internal interface ICollapsibleHeader : IDisposable
{
    public string HeaderText { get; }

    public void Draw()
    {
        if (ImGui.CollapsingHeader(HeaderText))
        {
            DrawContents();
        }
    }

    protected void DrawContents();
}