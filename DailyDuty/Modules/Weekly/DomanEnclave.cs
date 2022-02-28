using System.Numerics;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal class DomanEnclave : 
        IConfigurable,
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification
    {
        public void Dispose()
        {

        }

        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Doman Enclave";
        public GenericSettings GenericSettings => Settings;

        private readonly DalamudLinkPayload domanEnclaveTeleport;

        public DomanEnclave()
        {
            domanEnclaveTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.DomanEnclave);

        }

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            if (IsCompleted() == false)
            {
                Chat.Print(HeaderText, $"{Settings.Budget - Settings.CurrentEarnings:n0} gil remaining", domanEnclaveTeleport);
            }
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings, HeaderText);

            Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);

            Draw.Checkbox("Show Donation Amounts", HeaderText, ref Settings.ShowTrackedDonationAmount, "Display a message in chat with the donation amount recorded, useful for debugging");
        }

        public void EditModeOptions()
        {
            Draw.EditNumberField("Target Budget", HeaderText, 50, ref Settings.Budget);

            Draw.EditNumberField("Deposited This Week", HeaderText, 50, ref Settings.CurrentEarnings);
        }

        public void DisplayData()
        {
            ImGui.TextColored(new Vector4(0.8f, 0.2f, 0.2f, 1.0f),"Use Edit Mode to configure the target budget");

            Draw.NumericDisplay("Deposited This Week", Settings.CurrentEarnings);

            Draw.NumericDisplay("Target Budget", Settings.Budget);

            Draw.NumericDisplay("Remaining Budget", Settings.Budget - Settings.CurrentEarnings);
        }

        public bool IsCompleted()
        {
            return Settings.CurrentEarnings >= Settings.Budget;
        }
    }
}