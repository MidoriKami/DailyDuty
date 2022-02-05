using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using Dalamud.Interface.Components;
using ImGuiNET;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty.DisplaySystem.DisplayModules.Daily
{
    internal class Roulettes : DisplayModule
    {
        private ConfigurationSystem.Daily.Roulettes Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].RouletteSettings;
        
        protected override GenericSettings GenericSettings => Settings;

        public Roulettes()
        {
            CategoryString = "Roulettes";
        }

        protected override void DisplayData()
        {
            DisplayTrackingGrid();

            DisplayRouletteStatus();
        }

        private void DisplayRouletteStatus()
        {
            ImGui.Text("Tracked Roulette Status");

            if (ImGui.BeginTable($"##{CategoryString}Status", 2))
            {
                foreach (var tracked in Settings.TrackedRoulettes)
                {
                    if (tracked.Tracked == true)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text(tracked.Roulette.ToString());

                        ImGui.TableNextColumn();
                        PrintCompleteIncomplete(tracked.Completed);
                    }
                }

                ImGui.EndTable();
            }
        }

        private void DisplayTrackingGrid()
        {
            ImGui.Text("Select the Roulettes to track:");

            if (ImGui.BeginTable($"##{CategoryString}Table", 3))
            {
                foreach (var roulette in Settings.TrackedRoulettes)
                {
                    ImGui.TableNextColumn();
                    ImGui.Checkbox($"{roulette.Roulette}##{CategoryString}", ref roulette.Tracked);

                    if (roulette.Roulette == DutyRoulette.Mentor)
                    {
                        ImGui.SameLine();
                        ImGuiComponents.HelpMarker("You know it's going to be an extreme... right?");
                    }
                }

                ImGui.EndTable();
            }
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("Mark as Complete");

            if (ImGui.BeginTable($"##{CategoryString}EditTable", 3))
            {
                foreach (var roulette in Settings.TrackedRoulettes)
                {
                    if (roulette.Tracked == true)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Checkbox($"{roulette.Roulette}##{CategoryString}", ref roulette.Tracked);

                        ImGui.TableNextColumn();
                        if (ImGui.Button($"Mark Complete##{roulette.Roulette}{CategoryString}"))
                        {
                            roulette.Completed = true;
                            Service.Configuration.Save();
                        }

                        ImGui.TableNextColumn();
                        if (ImGui.Button($"Mark Incomplete##{roulette.Roulette}{CategoryString}"))
                        {
                            roulette.Completed = false;
                            Service.Configuration.Save();
                        }
                    }
                }

                ImGui.EndTable();
            }
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);

            OnTerritoryChangeCheckbox(Settings);
        }

        public override void Dispose()
        {
        }
    }
}
