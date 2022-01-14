using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal unsafe class WondrousTails : DisplayModule
    {
        protected readonly Daily.WondrousTailsSettings Settings = Service.Configuration.WondrousTailsSettings;

        public WondrousTails()
        {
            CategoryString = "Wondrous Tails";
        }

        protected override void DrawContents()
        {
            
        }

        public override void Dispose()
        {

        }
    }
}
