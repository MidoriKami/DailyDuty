using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Utilities;
using Dalamud.Utility.Signatures;
#pragma warning disable CS0169
#pragma warning disable CS0649

namespace DailyDuty.System.Modules
{
    internal unsafe class EliteHuntsModule : Module
    {
        private Weekly.EliteHuntSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].EliteHuntSettings;
        public override string ModuleName => "Elite Hunts";
        public override GenericSettings GenericSettings => Settings;

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private EliteHuntStruct* huntData;

        public EliteHuntsModule()
        {
            SignatureHelper.Initialise(this);
        }

        public override void Update()
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                var obtained = huntData->Obtained(hunt.Expansion);

                if (obtained == true)
                {
                    hunt.Obtained = true;
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

        protected override void ThrottledOnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                DisplayNotification();
            }
        }

        public override bool IsCompleted()
        {
            return CountUnclaimed() == 0;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.Obtained = false;
            }
        }

        private void DisplayNotification()
        {
            var unclaimed = CountUnclaimed();

            if(unclaimed > 0)
                Util.PrintEliteHunts($"You have {unclaimed} unclaimed elite marks.");
        }

        private int CountUnclaimed()
        {
            int count = 0;

            foreach (var hunt in Settings.TrackedHunts)
            {
                var obtained = huntData->Obtained(hunt.Expansion) || hunt.Obtained;

                if (!obtained && hunt.Tracked)
                    count++;
            }

            return count;
        }
    }
}
