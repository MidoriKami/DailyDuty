using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuizmoNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class FashionReport : DisplayModule
    {
        private Weekly.FashionReportSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].FashionReportSettings;

        protected override GenericSettings GenericSettings => Settings;

        private int modeSelect;

        public FashionReport()
        {
            CategoryString = "Fashion Report";
            modeSelect = (int) Settings.Mode;
        }

        protected override void DisplayData()
        {
            ImGui.Text("Remaining Allowances:");
            ImGui.SameLine();
            ImGui.Text($"{Settings.AllowancesRemaining}");


            ImGui.Text("Highest Score This Week:");
            ImGui.SameLine();
            ImGui.Text($"{Settings.HighestWeeklyScore}");

            ImGui.Text("Time Until Fashion Report:");
            ImGui.SameLine();

            var timeSpan = FashionReportModule.TimeUntilFashionReport();

            if (timeSpan == TimeSpan.Zero)
            {
                ImGui.TextColored(new(0, 255, 0, 255), $" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
            else
            {
                ImGui.Text($" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("Manually Set Values");

            EditNumberField("Allowances:", ref Settings.AllowancesRemaining);

            EditNumberField("Highest Score:", ref Settings.HighestWeeklyScore);
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);
            OnTerritoryChangeCheckbox(Settings);

            ImGui.Text("Notification Mode");


            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.RadioButton($"Single##{CategoryString}", ref modeSelect, (int)FashionReportMode.Single);
            ImGuiComponents.HelpMarker("Only notify if no allowances have been spent this week and fashion report is available for turn-in.");
            ImGui.SameLine();

                ImGui.Indent(125 * ImGuiHelpers.GlobalScale);

                ImGui.RadioButton($"All##{CategoryString}", ref modeSelect, (int) FashionReportMode.All);
                ImGuiComponents.HelpMarker("notify if any allowances remain this week and fashion report is available for turn-in.");

                ImGui.Indent(-125 * ImGuiHelpers.GlobalScale);

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);


            if (Settings.Mode != (FashionReportMode) modeSelect)
            {
                Settings.Mode = (FashionReportMode) modeSelect;
                Service.Configuration.Save();
            }
        }

        public override void Dispose()
        {
        }
    }
}
