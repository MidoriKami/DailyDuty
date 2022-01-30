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
            NumericDisplay("Tickets Remaining", Settings.TicketsRemaining);
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            EditNumberField("Override Ticket Count", ref Settings.TicketsRemaining);
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
