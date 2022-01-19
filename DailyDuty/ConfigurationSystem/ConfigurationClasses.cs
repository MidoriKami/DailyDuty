using System;
using System.Collections;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.ConfigurationSystem
{
    public class GenericSettings
    {
        public bool Enabled = false;
        public bool NotificationEnabled = false;
    }

    public enum ButtonState
    {
        // Can click button to get a stamp right now
        AvailableNow,

        // Needs instance completion to become available
        Completable,

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
        }
    }

    public class Weekly
    {
        public class WondrousTailsSettings : GenericSettings
        {
        }

        public class CustomDeliveriesSettings : GenericSettings
        {
            public uint AllowancesRemaining = 0;
        }
    }
}
