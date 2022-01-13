using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.CommandSystem.Commands
{
    internal class TreasureMapCommands : CommandProcessor
    {
        private readonly Configuration.DailyTreasureMapSettings settings = Service.Configuration.TreasureMapSettings;

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
                default:
                    break;
            }
        }
    }
}
