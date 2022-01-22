using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class WondrousTails : DisplayModule
    {
        protected Weekly.WondrousTailsSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings;

        public WondrousTails()
        {
            CategoryString = Loc.Localize("WT", "Wondrous Tails");
        }

        protected override void DrawContents()
        {
            var stringEnabled = Loc.Localize("Enabled", "Enabled");
            var stringNotifications = Loc.Localize("Notifications", "Notifications");
            var bookStatus = Loc.Localize("Book Status", "Book Status") + ": ";
            var stringComplete = Loc.Localize("Complete", "Complete");
            var stringIncomplete = Loc.Localize("Incomplete", "Incomplete");

            ImGui.Checkbox($"{stringEnabled}##WondrousTails", ref Settings.Enabled);
            ImGui.Spacing();

            if (Settings.Enabled)
            {
                ImGui.Indent(15 *ImGuiHelpers.GlobalScale);

                ImGui.Text(bookStatus);
                ImGui.SameLine();
                if(Settings.NumPlacedStickers == 9)
                {
                    ImGui.TextColored(new(0, 255, 0, 255), stringComplete);
                }
                else
                {
                    ImGui.TextColored(new(255, 0, 0, 100),stringIncomplete);
                }

                ImGui.Checkbox($"{stringNotifications}##WondrousTails", ref Settings.NotificationEnabled);
                ImGui.Spacing();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            ImGui.Spacing();
        }

        public override void Dispose()
        {

        }
    }
}
