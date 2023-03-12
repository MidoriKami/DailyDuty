using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models.Enums;
using KamiLib.Configuration;

namespace DailyDuty.Models.Modules;

public class TestModuleConfig : ModuleConfigBase
{
    
}

public class TestModuleData : ModuleDataBase
{
    public Setting<bool> moduleSpecific = new(true);
}

public class TestModule : Module.SpecialModule, IDisposable
{
    public override ModuleName ModuleName => ModuleName.TestModule;

    public void Dispose()
    {
    }
    
    public override DateTime GetNextReset()
    {
        return DateTime.UtcNow + TimeSpan.FromMinutes(1);
    }
    
    public override ModuleStatus GetModuleStatus()
    {
        return ModuleStatus.Unknown;
    }
    
    public override IStatusMessage GetStatusMessage()
    {
        return new StatusMessage
        {
            Message = "Butts",
            SourceModule = ModuleName
        };
    }
}