using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DailyDuty.System.Modules;
using DailyDuty.System.Modules.Daily;
using DailyDuty.System.Modules.Weekly;
using DailyDuty.System.Utilities;

namespace DailyDuty.System
{
    public class ModuleManager
    {
        private readonly Stopwatch resetDelayStopwatch = new();

        public enum ModuleType
        {
            Daily,
            Weekly
        }


        private readonly List<Module> dailyModules = new()
        {
            new TreasureMapModule(),
            new MiniCactpotModule(),
            new RoulettesModule()
        };

        private readonly List<Module> weeklyModules = new()
        {
            new WondrousTailsModule(),
            new CustomDeliveriesModule(),
            new FashionReportModule(),
            new JumboCactpotModule(),
            new EliteHuntsModule()
        };

        private List<Module> Modules =>
            dailyModules.Concat(weeklyModules).ToList();


        private readonly Queue<Module> updateQueue = new();

        public ModuleManager()
        {
            foreach (var module in Modules)
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
                foreach (var module in Modules)
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
                foreach (var module in Modules)
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
            foreach (var module in Modules)
            {
                module.Dispose();
            }
        }

        public IEnumerable<Module> GetModulesByType(ModuleType type)
        {
            if (type == ModuleType.Daily)
                return dailyModules;

            if (type == ModuleType.Weekly)
                return weeklyModules;

            throw new Exception("Invalid Module Type Used");
        }

        public bool TasksCompleteByType(ModuleType type)
        {
            return GetModulesByType(type)
                .Where(module => module.GenericSettings.Enabled)
                .All(module => module.IsCompleted());
        }
    }
}
