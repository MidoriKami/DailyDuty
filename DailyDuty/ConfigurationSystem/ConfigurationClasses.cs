using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;

namespace DailyDuty.ConfigurationSystem
{
    public class GenericSettings
    {
        public bool Enabled = false;
        public bool TerritoryChangeReminder = false;
        public bool LoginReminder = false;
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

    public enum FashionReportMode
    {
        Single,
        All
    }

    public class Daily
    {
        public class TreasureMapSettings : GenericSettings
        {
            public DateTime LastMapGathered = new();
            public int MinimumMapLevel = 0;
            public bool NotifyOnAcquisition = false;
            public bool HarvestableMapNotification = false;
        }

        public class Cactpot : GenericSettings
        {
            public int TicketsRemaining = 3;
        }
    }

    public class Weekly
    {
        public class WondrousTailsSettings : GenericSettings
        {
            public int NumPlacedStickers = 0;
            public bool InstanceStartNotification = false;
            public bool InstanceEndNotification = false;
            public bool RerollNotification = false;
        }

        public class CustomDeliveriesSettings : GenericSettings
        {
            public int AllowancesRemaining = 12;
        }

        public class JumboCactpotSettings : GenericSettings
        {
            public uint Datacenter = 0;
            public int UnclaimedTickets = 3;
            public int UnclaimedRewards = 0;
            public DateTime NextDrawing = new();
        }

        public class FashionReportSettings : GenericSettings
        {
            public int AllowancesRemaining = 4;
            public int HighestWeeklyScore = 0;
            public FashionReportMode Mode = FashionReportMode.Single;
            public DateTime FashionReportAvailableTime = new();
        }
    }
}
