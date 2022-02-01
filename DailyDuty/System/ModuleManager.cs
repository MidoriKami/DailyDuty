using System;
using System.Collections.Generic;
using System.Diagnostics;
using DailyDuty.System.Modules;
using DailyDuty.System.Utilities;

namespace DailyDuty.System
{
    internal class ModuleManager
    {
        private readonly Stopwatch ResetDelayStopwatch = new();

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

        public void Update()
        {
            Util.UpdateDelayed(ResetDelayStopwatch, TimeSpan.FromSeconds(1), UpdateResets);

            foreach (var (type, module) in modules)
            {
                module.Update();
            }
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
