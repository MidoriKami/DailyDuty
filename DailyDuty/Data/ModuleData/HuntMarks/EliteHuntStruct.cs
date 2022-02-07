using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct EliteHuntStruct
    {
        [FieldOffset(0x194)] private fixed byte ObtainedFlags[3];

        private bool ObtainedRealmReborn => (ObtainedFlags[0] & 0x10) != 0;
        private bool ObtainedHeavensward => (ObtainedFlags[0] & 0x20) != 0;
        private bool ObtainedStormblood => (ObtainedFlags[1] & 0x02) != 0;
        private bool ObtainedShadowbringers => (ObtainedFlags[1] & 0x20) != 0;
        private bool ObtainedEndwalker => (ObtainedFlags[2] & 0x02) != 0;

        public bool Obtained(ExpansionType expansion)
        {
            return expansion switch
            {
                ExpansionType.RealmReborn => ObtainedRealmReborn,
                ExpansionType.Heavensward => ObtainedHeavensward,
                ExpansionType.Stormblood => ObtainedStormblood,
                ExpansionType.Shadowbringers => ObtainedShadowbringers,
                ExpansionType.Endwalker => ObtainedEndwalker,
                _ => throw new Exception("[EliteHuntsModule] Unable to parse expansion input.")
            };
        }
    }
}
