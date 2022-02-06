using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
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
}
