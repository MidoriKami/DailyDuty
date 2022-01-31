using System;
using System.Numerics;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Interface;
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

            NumericDisplay("Unclaimed Rewards", Settings.UnclaimedRewards);

            DrawJumboCactpotWeeklyProgressBar();
        }

        private void DrawJumboCactpotWeeklyProgressBar()
        {
            var now = DateTime.UtcNow;
            var delta = TimeUntilNextDrawing();
            var percentage = (float)(1 - delta / TimeSpan.FromDays(7));

            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 255));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(176 / 255.0f, 38 / 255.0f, 236 / 255.0f, 0.5f));

            string daysDisplay = "";

            if (delta.Days == 1)
            {
                daysDisplay = $"{delta.Days} day, ";
            }
            else if (delta.Days > 1)
            {
                daysDisplay = $"{delta.Days} days, ";
            }

            ImGui.ProgressBar(percentage, ImGuiHelpers.ScaledVector2(200, 20), $"Next Drawing: {daysDisplay}{delta.Hours:00}:{delta.Minutes:00}:{delta.Seconds:00}");
            ImGui.PopStyleColor(2);
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            EditNumberField("Tickets Remaining", ref Settings.UnclaimedTickets);

            EditNumberField("Unclaimed Rewards", ref Settings.UnclaimedRewards);
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);

            OnTerritoryChangeCheckbox(Settings);
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
