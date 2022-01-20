using System.Collections.Generic;
using DailyDuty.System.Modules;

namespace DailyDuty.System
{
    internal class ModuleManager
    {
        public enum ModuleType
        {
            TreasureMap,
            WondrousTails,
            CustomDeliveries
        }

        private readonly Dictionary<ModuleType, Module> modules = new()
        {
            {ModuleType.TreasureMap, new TreasureMapModule()},
            {ModuleType.WondrousTails, new WondrousTailsModule()},
            {ModuleType.CustomDeliveries, new CustomDeliveriesModule()}
        };

        public void Update()
        {
            foreach (var (type, module) in modules)
            {
                module.Update();
            }
        }

        public void Dispose()
        {
            foreach (var (type, module) in modules)
            {
                module.Dispose();
            }
        }

        public Module this[ModuleType type] => GetModuleByType(type);

        public Module GetModuleByType(ModuleManager.ModuleType type)
        {
            return modules[type];
        }
    }
}
