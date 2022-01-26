using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyDuty.ConfigurationSystem
{
    public class GenericSettings
    {
        public bool Enabled = false;
    }

    public enum ButtonState
    {
        // Needs instance completion to become available
        Completable,

        // Can click button to get a stamp right now
        AvailableNow,

        // Already completed, needs re-roll
        Unavailable,

        // Data is state, unknown state
        Unknown
    }

    public class Daily
    {
        public class TreasureMapSettings : GenericSettings
        {
            public DateTime LastMapGathered = new();
            public int MinimumMapLevel = 0;
            public bool NotifyOnAcquisition = false;
            public bool HarvestableMapNotification = false;
            public bool PersistentReminders = false;
        }

        public class Cactpot : GenericSettings
        {
            public int TicketsRemaining = 3;
            public bool PersistentReminders = false;
        }
    }

    public class Weekly
    {
        public class WondrousTailsSettings : GenericSettings
        {
            public int NumPlacedStickers = 0;
            public bool InstanceNotification = false;
        }

        public class CustomDeliveriesSettings : GenericSettings
        {
            public uint AllowancesRemaining => (uint)(12 - DeliveryNPC.Sum(r => 6 - r.Value));
            public bool PersistentReminders = false;

            public Dictionary<uint, uint> DeliveryNPC = new();
        }
    }
}
