using System;
using Dalamud.Logging;

namespace DailyDuty.Utilities.Helpers.JumboCactpot
{
    internal static class DatacenterLookup
    {
        public static DateTime GetDrawingTime(uint datacenter)
        {
            switch (datacenter)
            {
                // Elemental, Gaia, Mana
                case 1 or 2 or 3:
                    return Time.NextDayOfWeek(DayOfWeek.Saturday, 12);

                // Aether, Primal, Crystal
                case 4 or 5 or 8:
                    return Time.NextDayOfWeek(DayOfWeek.Sunday, 2);

                // Chaos, Light
                case 6 or 7:
                    return Time.NextDayOfWeek(DayOfWeek.Saturday, 19);

                // Materia
                case 9:
                    return Time.NextDayOfWeek(DayOfWeek.Saturday, 9);
            }

            PluginLog.Error($"[Util] Unable to determine DataCenter: {datacenter}");
            return new();
        }
        
        public static uint? TryGetPlayerDatacenter()
        {
            if (Service.LoggedIn == false) return null;

            var player = Service.ClientState.LocalPlayer;
            if (player == null) return null;

            var gameData = player.HomeWorld.GameData;
            if(gameData == null) return null;

            return gameData.DataCenter.Row;
        }
    }
}
