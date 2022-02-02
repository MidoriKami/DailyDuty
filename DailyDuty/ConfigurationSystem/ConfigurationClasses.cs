using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data;
using DailyDuty.DisplaySystem.DisplayModules;
using DailyDuty.System.Modules;
using Dalamud.Game;

namespace DailyDuty.ConfigurationSystem
{
    public class GenericSettings
    {
        public bool Enabled = false;
        public bool TerritoryChangeReminder = false;
        public bool LoginReminder = false;
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
            public bool RerollNotificationStickers = false;
            public bool RerollNotificationTasks = false;
            public DateTime CompletionDate = new();
            public bool NewBookNotification = false;
            public ushort WeeklyKey = 0;
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
        }

        public class EliteHuntSettings : GenericSettings
        {
            public (EliteHuntEnum, bool, bool)[] EliteHunts = new (EliteHuntEnum, bool, bool)[5]
            {
                new(EliteHuntEnum.RealmReborn, false, false),
                new(EliteHuntEnum.Heavensward, false, false),
                new(EliteHuntEnum.Stormblood, false, false),
                new(EliteHuntEnum.Shadowbringers, false, false),
                new(EliteHuntEnum.Endwalker, false, false)
            };
        }
    }
}
