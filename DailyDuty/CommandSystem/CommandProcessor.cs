using System.Collections.Generic;

namespace DailyDuty.CommandSystem
{
    internal abstract class CommandProcessor
    {
        protected List<string> PrimaryCommandFilters = new();

        public void ProcessCommand(string primaryCommand, string? secondaryCommand)
        {
            foreach (var filter in PrimaryCommandFilters)
            {
                if (primaryCommand == filter)
                {
                    ProcessSecondaryCommand(secondaryCommand);
                }
            }
        }

        private void ProcessSecondaryCommand(string? secondaryCommand)
        {
            switch (secondaryCommand)
            {
                case null:
                    ProcessNullSecondaryCommand();
                    break;

                case "on":
                    ProcessOnCommand();
                    break;

                case "off":
                    ProcessOffCommand();
                    break;

                default:
                    ProcessCustomCommand(secondaryCommand);
                    break;
            }
        }

        protected abstract void ProcessNullSecondaryCommand();
        protected abstract void ProcessOnCommand();
        protected abstract void ProcessOffCommand();
        protected abstract void ProcessCustomCommand(string secondaryCommand);
    }
}
