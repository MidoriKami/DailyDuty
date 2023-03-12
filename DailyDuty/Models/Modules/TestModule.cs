using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models.Enums;
using KamiLib.Configuration;

namespace DailyDuty.Models.Modules;

public class TestModuleConfig : ModuleConfigBase
{
    public string Test = "Did it work??";
}

public class TestModuleData : ModuleDataBase
{
    public Setting<bool> moduleSpecific = new(true);
}

public class TestModule : Module.SpecialModule, IDisposable
{
    public override ModuleDataBase ModuleData { get; protected set; } = new TestModuleData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TestModuleConfig();
    public override ModuleName ModuleName => ModuleName.TestModule;

    public void Dispose()
    {
    }

    public override void Update()
    {
        base.Update();
        var data = ModuleData as TestModuleData ?? new TestModuleData();
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