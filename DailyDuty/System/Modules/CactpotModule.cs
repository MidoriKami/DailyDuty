using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;

namespace DailyDuty.System.Modules
{
    internal class CactpotModule : Module
    {
        private Daily.Cactpot Settings => Service.Configuration
            .CharacterSettingsMap[Service.Configuration.CurrentCharacter].CactpotSettings;

        public override void Dispose()
        {
        }

        protected override void OnLoginDelayed()
        {
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
        }

        public override bool IsCompleted()
        {
            return Settings.TicketsRemaining == 0;
        }

        public override void DoDailyReset()
        {
        }

        public override void DoWeeklyReset()
        {
        }
    }
}
