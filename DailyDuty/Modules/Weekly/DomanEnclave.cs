using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal class DomanEnclave : ICollapsibleHeader, IUpdateable
    {
        public void Dispose()
        {

        }

        public string HeaderText { get; } = "Doman Enclave";
        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Not Implemented Yet");
        }

        public void Update()
        {
            
        }
    }
}
