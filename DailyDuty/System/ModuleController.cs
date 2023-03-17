using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models.Enums;

namespace DailyDuty.System;

public class ModuleController : IDisposable
{
    private readonly List<BaseModule> modules;
    private readonly GoldSaucerMessageController goldSaucerMessageController;

    public ModuleController()
    {
        modules = new List<BaseModule>();
        goldSaucerMessageController = new GoldSaucerMessageController();

        foreach (var t in GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseModule))))
        {
            if (t.IsAbstract) continue;
            var module = (BaseModule?) Activator.CreateInstance(t);
            if (module is null) continue;
            
            modules.Add(module);
        }

        goldSaucerMessageController.GoldSaucerUpdate += OnGoldSaucerMessage;
    }
    
    public void Dispose()
    {
        goldSaucerMessageController.GoldSaucerUpdate -= OnGoldSaucerMessage;
        goldSaucerMessageController.Dispose();

        foreach (var module in modules.OfType<IDisposable>())
        {
            module.Dispose();
        }
    }

    public IEnumerable<BaseModule> GetModules(ModuleType? type = null) => 
        type is null ? modules : modules.Where(module => module.ModuleType == type);

    public void UpdateModules()
    {
        foreach (var module in modules)
        {
            module.Update();
        }
    }

    public void LoadModules()
    {
        foreach (var module in modules)
        {
            module.Load();
        }
    }
    
    public void UnloadModules()
    {
        foreach (var module in modules)
        {
            module.Unload();
        }
    }

    public void ResetModules()
    {
        var now = DateTime.UtcNow;
        
        foreach (var module in modules)
        {
            if (now >= module.ModuleData.NextReset)
            {
                module.Reset();
            }
        }
    }

    public void ZoneChange(uint newZone)
    {
        foreach (var module in modules)
        {
            module.ZoneChange(newZone);
        }
    }

    public void AddonSetup(SetupAddonArgs addonInfo)
    {
        foreach(var module in modules)
        {
            module.AddonSetup(addonInfo);
        }
    }
    
    private void OnGoldSaucerMessage(object? sender, GoldSaucerEventArgs e)
    {
        foreach (var module in modules.OfType<IGoldSaucerMessageReceiver>())
        {
            module.GoldSaucerUpdate(sender, e);
        }
    }

}