using System;
using System.Diagnostics;
using DailyDuty.DataModels;
using Dalamud.Game;
using Dalamud.Logging;

namespace DailyDuty.System;

public class ResetManager : IDisposable
{
    private readonly Stopwatch timer = Stopwatch.StartNew();
    
    public ResetManager()
    {
        Service.Framework.Update += FrameworkOnUpdate;
    }

    private void FrameworkOnUpdate(Framework framework)
    {
        if (timer.Elapsed.TotalSeconds > 5)
        {
            ResetModules();

            timer.Restart();
        }
    }

    public static void ResetModules()
    {
        if (Service.ConfigurationManager.CharacterDataLoaded)
        {
            var anyModulesReset = false;
            
            foreach (var module in Service.ModuleManager.GetLogicComponents())
            {
                var now = DateTime.UtcNow;

                if (now >= module.ParentModule.GenericSettings.NextReset)
                {
                    module.Reset();
                    module.ParentModule.GenericSettings.NextReset = module.GetNextReset();

                    anyModulesReset = true;
                    PluginLog.Debug($"[{module.ParentModule.Name.GetTranslatedString()}] performing reset. Next Reset:[{module.ParentModule.GenericSettings.NextReset.ToLocalTime()}]");
                }
            }

            if (anyModulesReset)
            {
                Service.ConfigurationManager.Save();
            }
        }
    }

    public void Dispose()
    {
        Service.Framework.Update += FrameworkOnUpdate;
    }
}