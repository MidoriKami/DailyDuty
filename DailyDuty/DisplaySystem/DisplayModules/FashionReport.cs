using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

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
            NumericDisplay("Remaining Allowances", Settings.AllowancesRemaining);

            NumericDisplay("Highest Score This Week", Settings.HighestWeeklyScore);

            DaysTimeSpanDisplay("Time Until Fashion Report", FashionReportModule.TimeUntilFashionReport() );
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
