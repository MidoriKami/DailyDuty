using DailyDuty.ConfigurationSystem;
using System;
using System.Runtime.InteropServices;

namespace DailyDuty.Data
{
    public enum EliteHuntExpansionEnum
    {
        RealmReborn,
        Heavensward,
        Stormblood,
        Shadowbringers,
        Endwalker
    }

    public class TrackedHunt
    {
        public EliteHuntExpansionEnum Expansion;
        public bool Tracked;
        public bool UpdatedThisWeek;

        public TrackedHunt(EliteHuntExpansionEnum expansion, bool tracked, bool updated)
        {
            Expansion = expansion;
            Tracked = tracked;
            UpdatedThisWeek = updated;
        }
    }

    public struct HuntStatus
    {
        public bool Obtained = false;
        public bool Killed = false;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct EliteHuntStruct
    {
        [FieldOffset(0x7C)] private readonly byte ARR_Killed_Flag;

        [FieldOffset(0x90)] private readonly byte Heavensward_Killed_Flag;

        [FieldOffset(0xE0)] private readonly byte Stormblood_Killed_Flag;

        [FieldOffset(0x130)] private readonly byte Shadowbringers_Killed_Flag;

        [FieldOffset(0x180)] private readonly byte Endwalker_Killed_Flag;

        [FieldOffset(0x194)] private fixed byte ObtainedFlags[3];

        private bool ObtainedRealmReborn => (ObtainedFlags[0] & 0x10) != 0;
        private bool ObtainedHeavensward => (ObtainedFlags[0] & 0x20) != 0;
        private bool ObtainedStormblood => (ObtainedFlags[1] & 0x02) != 0;
        private bool ObtainedShadowbringers => (ObtainedFlags[1] & 0x20) != 0;
        private bool ObtainedEndwalker => (ObtainedFlags[2] & 0x02) != 0;

        private bool KilledRealmReborn => (ARR_Killed_Flag & 0x01) != 0;
        private bool KilledHeavensward => (Heavensward_Killed_Flag & 0x01) != 0;
        private bool KilledStormblood => (Stormblood_Killed_Flag & 0x01) != 0;
        private bool KilledShadowbringers => (Shadowbringers_Killed_Flag & 0x01) != 0;
        private bool KilledEndwalker => (Endwalker_Killed_Flag & 0x01) != 0;

        public HuntStatus GetStatus(EliteHuntExpansionEnum expansion)
        {
            return expansion switch
            {
                EliteHuntExpansionEnum.RealmReborn => new HuntStatus {Killed = KilledRealmReborn, Obtained = ObtainedRealmReborn},
                EliteHuntExpansionEnum.Heavensward => new HuntStatus {Killed = KilledHeavensward, Obtained = ObtainedHeavensward},
                EliteHuntExpansionEnum.Stormblood => new HuntStatus {Killed = KilledStormblood, Obtained = ObtainedStormblood},
                EliteHuntExpansionEnum.Shadowbringers => new HuntStatus {Killed = KilledShadowbringers, Obtained = ObtainedShadowbringers},
                EliteHuntExpansionEnum.Endwalker => new HuntStatus {Killed = KilledEndwalker, Obtained = ObtainedEndwalker},
                _ => throw new Exception("[EliteHuntsModule] Unable to parse expansion input.")
            };
        }

        public void PrintDebug()
        {
            Service.Chat.Print($"{ARR_Killed_Flag:x2} :: {ObtainedRealmReborn:x2}");
            Service.Chat.Print($"{Heavensward_Killed_Flag:x2} :: {ObtainedHeavensward:x2}");
            Service.Chat.Print($"{Stormblood_Killed_Flag:x2} :: {ObtainedStormblood:x2}");
            Service.Chat.Print($"{Shadowbringers_Killed_Flag:x2} :: {ObtainedShadowbringers:x2}");
            Service.Chat.Print($"{Endwalker_Killed_Flag:x2} :: {ObtainedEndwalker:x2}");

        }
    }
}