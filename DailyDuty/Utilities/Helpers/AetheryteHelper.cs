using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using Dalamud.Game.ClientState.Aetherytes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.Utilities.Helpers
{
    internal static class AetheryteHelper
    {
        public static AetheryteEntry Get(TeleportLocation location)
        {
            return location switch
            {
                TeleportLocation.GoldSaucer => GetByID(62),
                TeleportLocation.Idyllshire => GetByID(75),
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
