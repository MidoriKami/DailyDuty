using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class OverlaysConfiguration : ICollapsibleHeader
    {
        private readonly List<ICollapsibleHeader> headers = new()
        {
            new DutyFinderOverlayConfiguration(),
        };

        public void Dispose()
        {

        }

        public string HeaderText => "Overlay Configurations";
        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            foreach (var header in headers)
            {
                header.Draw();
            }

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }
    }
}
