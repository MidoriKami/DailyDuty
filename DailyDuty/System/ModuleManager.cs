using System;
using System.Collections.Generic;
using System.Diagnostics;
using DailyDuty.System.Modules;
using DailyDuty.System.Utilities;

namespace DailyDuty.System
{
    public class ModuleManager
    {
        private readonly Stopwatch resetDelayStopwatch = new();

        public enum ModuleType
        {
            TreasureMap,
            WondrousTails,
            CustomDeliveries,
            MiniCactpot,
            FashionReport,
            JumboCactpot,
            EliteHunts
        }

        private readonly Dictionary<ModuleType, Module> modules = new()
        {
            {ModuleType.TreasureMap, new TreasureMapModule()},
            {ModuleType.WondrousTails, new WondrousTailsModule()},
            {ModuleType.CustomDeliveries, new CustomDeliveriesModule()},
            {ModuleType.MiniCactpot, new MiniCactpotModule()},
            {ModuleType.FashionReport, new FashionReportModule()},
            {ModuleType.JumboCactpot, new JumboCactpotModule()},
            {ModuleType.EliteHunts, new EliteHuntsModule()}
        };

        private readonly Queue<Module> updateQueue = new();

        public ModuleManager()
        {
            foreach (var module in modules.Values)
            {
                updateQueue.Enqueue(module);
            }
        }

        public void Update()
        {
            Util.UpdateDelayed(resetDelayStopwatch, TimeSpan.FromSeconds(1), UpdateResets);

            var module = updateQueue.Dequeue();

            module.Update();

            updateQueue.Enqueue(module);
        }

        private void UpdateResets()
        {
            UpdateDailyReset();

            UpdateWeeklyReset();
        }

        private void UpdateDailyReset()
        {
            if (DateTime.UtcNow > Service.Configuration.NextDailyReset)
            {
                foreach (var (_, module) in modules)
                {
                    foreach (var (_, settings) in Service.Configuration.CharacterSettingsMap)
                    {
                        module.DoDailyReset(settings);
                    }
                }

                Service.Configuration.NextDailyReset = Util.NextDailyReset();
                Service.Configuration.Save();
            }
        }

        private void UpdateWeeklyReset()
        {
            if (DateTime.UtcNow > Service.Configuration.NextWeeklyReset)
            {
                foreach (var (_, module) in modules)
                {
                    foreach (var (_, settings) in Service.Configuration.CharacterSettingsMap)
                    {
                        module.DoWeeklyReset(settings);
                    }
                }

                Service.Configuration.NextWeeklyReset = Util.NextWeeklyReset();
                Service.Configuration.Save();
            }
        }

        public void Dispose()
        {
            foreach (var (_, module) in modules)
            {
                module.Dispose();
            }
        }

        public Module this[ModuleType type] => GetModuleByType(type);

        public Module GetModuleByType(ModuleType type)
        {
            return modules[type];
        }
    }
}
