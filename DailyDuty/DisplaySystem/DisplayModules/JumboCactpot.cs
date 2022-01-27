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
            ImGui.Text($"Tickets Remaining: {Settings.UnclaimedTickets}");

            ImGui.Text($"Claimed Tickets: {Settings.ClaimedTickets}");

            ImGui.Text($"Claimed Rewards: {Settings.ClaimedRewards}");

            DisplayTimeUntilNextDrawing();
        }

        private void DisplayTimeUntilNextDrawing()
        {
            var delta = TimeUntilNextDrawing();

            ImGui.Text("Time Until Next Drawing:");
            ImGui.SameLine();

            string daysDisplay = "";

            if (delta.Days == 1)
            {
                daysDisplay = $"{delta.Days} day, ";
            }
            else if (delta.Days > 1)
            {
                daysDisplay = $"{delta.Days} days, ";
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
