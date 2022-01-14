using System.Collections.Generic;
using DailyDuty.System.Modules;
using Dalamud.Game;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty.System
{
    internal class ModuleManager
    {
        private readonly List<Module> modules = new()
        {
            new TreasureMapModule(),
            new WondrousTailsModule()
        };

        public ModuleManager()
        {

        }

        public void Update()
        {
            foreach (var module in modules)
            {
                module.Update();
            }
        }

        public void Dispose()
        {
            foreach (var module in modules)
            {
                module.Dispose();
            }
        }
    }
}
