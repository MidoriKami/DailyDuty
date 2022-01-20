using DailyDuty.ConfigurationSystem;

namespace DailyDuty.CommandSystem.Commands
{
    internal class TreasureMapCommands : CommandProcessor
    {
        private Daily.TreasureMapSettings settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings;

        public TreasureMapCommands()
        {
            PrimaryCommandFilters = new()
            {
                "tmap",
                "map",
                "treasure",
                "maps"
            };
        }

        protected override void ProcessNullSecondaryCommand()
        {
            settings.Enabled = !settings.Enabled;
        }

        protected override void ProcessOnCommand()
        {
            settings.Enabled = true;
        }

        protected override void ProcessOffCommand()
        {
            settings.Enabled = false;
        }

        protected override void ProcessCustomCommand(string secondaryCommand)
        {
            switch (secondaryCommand)
            {
                case "notify":
                    settings.NotificationEnabled = true;
                    break;

                case "silence":
                    settings.NotificationEnabled = false;
                    break;

                default:
                    break;
            }
        }
    }
}
