using System;

namespace DailyDuty.ConfigurationSystem
{
    public class GenericSettings
    {
        public bool Enabled = false;
        public bool NotificationEnabled = false;
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
        }
    }

    public class Weekly
    {
        public class WondrousTailsSettings : GenericSettings
        {
            public int NumPlacedStickers = 0;
        }

        public class CustomDeliveriesSettings : GenericSettings
        {
            public uint AllowancesRemaining = 12;
        }
    }
}
