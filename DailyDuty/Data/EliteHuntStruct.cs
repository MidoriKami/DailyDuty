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
        public bool Obtained;

        public TrackedHunt(EliteHuntExpansionEnum expansion, bool tracked)
        {
            Expansion = expansion;
            Tracked = tracked;
            Obtained = false;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct EliteHuntStruct
    {
        [FieldOffset(0x194)] private fixed byte ObtainedFlags[3];

        private bool ObtainedRealmReborn => (ObtainedFlags[0] & 0x10) != 0;
        private bool ObtainedHeavensward => (ObtainedFlags[0] & 0x20) != 0;
        private bool ObtainedStormblood => (ObtainedFlags[1] & 0x02) != 0;
        private bool ObtainedShadowbringers => (ObtainedFlags[1] & 0x20) != 0;
        private bool ObtainedEndwalker => (ObtainedFlags[2] & 0x02) != 0;

        public bool Obtained(EliteHuntExpansionEnum expansion)
        {
            return expansion switch
            {
                EliteHuntExpansionEnum.RealmReborn => ObtainedRealmReborn,
                EliteHuntExpansionEnum.Heavensward => ObtainedHeavensward,
                EliteHuntExpansionEnum.Stormblood => ObtainedStormblood,
                EliteHuntExpansionEnum.Shadowbringers => ObtainedShadowbringers,
                EliteHuntExpansionEnum.Endwalker => ObtainedEndwalker,
                _ => throw new Exception("[EliteHuntsModule] Unable to parse expansion input.")
            };
        }
    }
}