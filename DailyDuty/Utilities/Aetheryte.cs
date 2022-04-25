using System;
using System.Linq;
using DailyDuty.Enums;
using Dalamud.Game.ClientState.Aetherytes;

namespace DailyDuty.Utilities
{
    internal static class Aetheryte
    {
        public static AetheryteEntry Get(TeleportLocation location)
        {
            return location switch
            {
                TeleportLocation.GoldSaucer => GetByID(62),
                TeleportLocation.Idyllshire => GetByID(75),
                TeleportLocation.DomanEnclave => GetByID(127),
                _ => throw new Exception("Invalid TeleportLocation Requested")
            };
        }

        private static AetheryteEntry GetByID(uint id)
        {
            return Service.AetheryteList
                .Where(e => e.AetheryteData.GameData?.RowId == id)
                .First();
        }
    }
}
