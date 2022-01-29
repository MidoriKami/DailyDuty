using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class MiniCactpot : DisplayModule
    {
        protected Daily.Cactpot Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].MiniCactpotSettings;

        protected override GenericSettings GenericSettings => Settings;

        public MiniCactpot()
        {
            CategoryString = "Mini Cactpot";
        }

        protected override void DisplayData()
        {
            ImGui.Text($"Tickets Remaining:\t{Settings.TicketsRemaining}");
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("Override Ticket Count:");

            ImGui.SameLine();
            
            ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);

            if (ImGui.InputInt($"##{CategoryString}", ref Settings.TicketsRemaining, 0, 0))
            {
                if (Settings.TicketsRemaining > 3)
                {
                    Settings.TicketsRemaining = 3;
                }
                else if (Settings.TicketsRemaining < 0)
                {
                    Settings.TicketsRemaining = 0;
                }

                Service.Configuration.Save();
            }

            ImGui.PopItemWidth();
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
