﻿using System;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal abstract class DisplayModule : IDisposable
    {
        public string CategoryString = "CategoryString Not Set";

        protected abstract void DrawContents();

        public virtual void Draw()
        {
            ImGui.Text(CategoryString);
            ImGui.Spacing();

            DrawContents();

            ImGui.Spacing();
        }

        public abstract void Dispose();
    }
}
