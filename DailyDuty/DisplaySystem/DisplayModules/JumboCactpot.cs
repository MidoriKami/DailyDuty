using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class JumboCactpot : DisplayModule
    {
        private Weekly.JumboCactpotSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].JumboCactpotSettings;

        protected override GenericSettings GenericSettings => Settings;

        public JumboCactpot()
        {
            CategoryString = Loc.Localize("Jumbo Cactpot", "Jumbo Cactpot");
        }

        protected override void DisplayData()
        {
            var locString = Loc.Localize("Unclaimed Tickets Remaining", "Tickets Remaining: {0}").Format(Settings.UnclaimedTickets);
            ImGui.Text(locString);

            var claimedString = Loc.Localize("Claimed Tickets", "Claimed Tickets: {0}").Format(Settings.ClaimedTickets);
            ImGui.Text(claimedString);

            var claimedRewardsString = Loc.Localize("Claimed Rewards", "Claimed Rewards: {0}").Format(Settings.ClaimedRewards);
            ImGui.Text(claimedRewardsString);

            DisplayTimeUntilNextDrawing();
        }

        private void DisplayTimeUntilNextDrawing()
        {
            var daysString = Loc.Localize("days", "days");
            var dayString = Loc.Localize("day", "day");
            var delta = TimeUntilNextDrawing();

            ImGui.Text(Loc.Localize("JumboCactpotTimeUntilNextDrawing", "Time Until Next Drawing: "));
            ImGui.SameLine();

            string daysDisplay = "";

            if (delta.Days == 1)
            {
                daysDisplay = $"{delta.Days} {dayString}, ";
            }
            else if (delta.Days > 1)
            {
                daysDisplay = $"{delta.Days} {daysString}, ";
            }

            if (delta == TimeSpan.Zero)
            {
                ImGui.TextColored(new(0, 255, 0, 255), $"{delta.Hours:00}:{delta.Minutes:00}:{delta.Seconds:00}");
            }
            else
            {
                ImGui.Text($"{daysDisplay}{delta.Hours:00}:{delta.Minutes:00}:{delta.Seconds:00}");
            }
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
