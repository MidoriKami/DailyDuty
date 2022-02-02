using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
#pragma warning disable CS0169
#pragma warning disable CS0649

namespace DailyDuty.System.Modules
{
    internal unsafe class EliteHuntsModule : Module
    {
        private Weekly.EliteHuntSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].EliteHuntSettings;

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private EliteHuntStruct* huntData;

        private readonly Stopwatch delayStopwatch = new();

        public EliteHuntsModule()
        {
            SignatureHelper.Initialise(this);
        }

        public override void Update()
        {
            if (Settings.Enabled == false) return;

            Util.UpdateDelayed(delayStopwatch, TimeSpan.FromSeconds(5), UpdateHuntObtain );
        }

        private void UpdateHuntObtain()
        {
            foreach (var hunt in Settings.EliteHunts)
            {
                var huntStatus = huntData->GetStatus(hunt.Expansion);

                if (huntStatus.Obtained == true && hunt.UpdatedThisWeek == false)
                {
                    hunt.UpdatedThisWeek = true;
                }
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                DisplayNotification();
            }
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                DisplayNotification();
            }
        }

        public override bool IsCompleted()
        {
            var unkilled = CountUnkilled();

            return unkilled == 0;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            foreach (var hunt in Settings.EliteHunts)
            {
                hunt.UpdatedThisWeek = false;
            }
        }

        private void DisplayNotification()
        {
            var unclaimed = CountUnclaimed();
            var unkilled = CountUnkilled();

            if(unclaimed > 0)
                Util.PrintEliteHunts($"You have {unclaimed} unclaimed elite marks.");

            if(unkilled > 0)
                Util.PrintEliteHunts($"You have {unkilled} claimed but unkilled elite marks.");

        }

        private int CountUnclaimed()
        {
            int count = 0;

            foreach (var hunt in Settings.EliteHunts)
            {
                var huntStatus = huntData->GetStatus(hunt.Expansion);

                if (hunt.Tracked && huntStatus.Obtained == false && hunt.UpdatedThisWeek == false)
                    count++;
            }

            return count;
        }

        private int CountUnkilled()
        {
            int count = 0;

            foreach (var hunt in Settings.EliteHunts)
            {
                var huntStatus = huntData->GetStatus(hunt.Expansion);

                if (hunt.Tracked && huntStatus.Killed == false && huntStatus.Obtained == true && hunt.UpdatedThisWeek)
                    count++;
            }

            return count;
        }
    }
}
