﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class WondrousTails : DisplayModule
    {
        protected readonly Weekly.WondrousTailsSettings Settings = Service.Configuration.WondrousTailsSettings;

        public WondrousTails()
        {
            CategoryString = "Wondrous Tails";
        }

        protected override void DrawContents()
        {
            ImGui.Checkbox("Enabled##WondrousTails", ref Settings.Enabled);
            ImGui.Spacing();

            if (Settings.Enabled)
            {
                ImGui.Indent(15 *ImGuiHelpers.GlobalScale);

                ImGui.Checkbox("Notifications##WondrousTails", ref Settings.NotificationEnabled);
                ImGui.Spacing();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            ImGui.Spacing();
        }

        public override void Dispose()
        {

        }
    }
}
