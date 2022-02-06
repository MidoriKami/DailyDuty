using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Daily;
using DailyDuty.Modules.Weekly;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Utility;

namespace DailyDuty.System
{
    public class ModuleManager : IDisposable
    {
        private readonly List<object> modules = new()
        {
            // Daily
            new MiniCactpot(),
            new BeastTribe(),
            new DutyRoulette(),
            new GrandCompany(),
            new Levequests(),
            new TreasureMap(),

            // Weekly
            new BlueMageLog(),
            new ChallengeLog(),
            new CustomDelivery(),
            new DomanEnclave(),
            new FashionReport(),
            new HuntMarks(),
            new JumboCactpot(),
            new MaskedCarnival(),
            new WondrousTails()
        };

        private readonly Queue<IUpdateable> updateQueue;
        private readonly Stopwatch resetDelayStopwatch = new();
        private int zoneChangeCounter;

        public ModuleManager()
        {
            updateQueue = new(modules.OfType<IUpdateable>());

            Service.Framework.Update += Update;
            Service.ClientState.Login += PreOnLogin;
            Service.ClientState.TerritoryChanged += PreOnTerritoryChanged;
        }

        private void PreOnTerritoryChanged(object? sender, ushort e)
        {
            if (Service.LoggedIn == false) return;

            AlwaysOnTerritoryChanged(sender, e);

            zoneChangeCounter++;
            if (zoneChangeCounter % Service.Configuration.System.ZoneChangeDelayRate != 0) return;

            ThrottledOnTerritoryChanged(sender, e);
        }

        private void ThrottledOnTerritoryChanged(object? sender, ushort @ushort)
        {
            foreach (var module in modules.OfType<IZoneChangeThrottledNotification>())
            {
                module.TrySendNotification();
            }
        }

        private void AlwaysOnTerritoryChanged(object? sender, ushort @ushort)
        {
            foreach (var module in modules.OfType<IZoneChangeAlwaysNotification>())
            {
                module.TrySendNotification();
            }
        }

        private void PreOnLogin(object? sender, EventArgs e)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(task => OnLoginDelayed());
        }

        private void OnLoginDelayed()
        {
            foreach (var module in modules.OfType<ILoginNotification>())
            {
                module.TrySendNotification();
            }
        }

        private void Update(Framework framework)
        {
            Time.UpdateDelayed(resetDelayStopwatch, TimeSpan.FromSeconds(1), UpdateResets);

            var module = updateQueue.Dequeue();

            module.Update();

            updateQueue.Enqueue(module);
        }

        private void UpdateResets()
        {
            foreach (var resettable in modules.OfType<IResettable>())
            {
                if (!resettable.NeedsResetting()) continue;

                foreach (var characterSettings in Service.Configuration.CharacterSettingsMap.Values)
                {
                    resettable.DoReset(characterSettings);
                }
            }
        }

        public void Dispose()
        {
            Service.Framework.Update -= Update;
            Service.ClientState.Login -= PreOnLogin;
            Service.ClientState.TerritoryChanged -= PreOnTerritoryChanged;
        }
    }
}
