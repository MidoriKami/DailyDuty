using System;
using DailyDuty.ConfigurationSystem;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class JumboCactpot : DisplayModule
    {
        private Weekly.JumboCactpotSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].JumboCactpotSettings;

        protected override GenericSettings GenericSettings => Settings;

        public JumboCactpot()
        {
            CategoryString = "Jumbo Cactpot";
        }

        protected override void DisplayData()
        {
            NumericDisplay("Tickets Remaining", Settings.UnclaimedTickets);

            NumericDisplay("Claimed Tickets", Settings.ClaimedTickets);

            NumericDisplay("Claimed Rewards", Settings.ClaimedRewards);

            DaysTimeSpanDisplay("Time Until Next Drawing", TimeUntilNextDrawing());
        }
        
        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
        }

        protected override void NotificationOptions()
        {
        }

        public override void Dispose()
        {
        }

        private TimeSpan TimeUntilNextDrawing()
        {
            var nextMapTime = Settings.NextDrawing;

            if (DateTime.UtcNow >= nextMapTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return nextMapTime - DateTime.UtcNow;
            }
        }
    }
}
