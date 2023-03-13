using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;

namespace DailyDuty.Models.Modules;

public class TestModuleConfig : ModuleConfigBase
{
    [ConfigOption("Test String")]
    public string Test = "Did it work??";

    [ConfigOption("Test String")]
    public string Test2 = "Did it work??";
    
    [ConfigOption("Test String")]
    public string Test3 = "Did it work??";
    
    [ConfigOption("Test String")]
    public string Test4 = "Did it work??";
    
    [ConfigOption("Test String")]
    public string Test5 = "Did it work??";
    
    [ConfigOption("Test String")]
    public string Test6 = "Did it work??";

    [ConfigOption("Another Option")]
    public bool Another = true;
}

public class TestModuleData : ModuleDataBase
{
    public bool Specificthing = true;
}

public class TestModule : Module.SpecialModule
{
    public override ModuleDataBase ModuleData { get; protected set; } = new TestModuleData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TestModuleConfig();
    public override ModuleName ModuleName => ModuleName.TestModule;
    public override DateTime GetNextReset() => DateTime.UtcNow + TimeSpan.FromMinutes(5);
    public override ModuleStatus GetModuleStatus() => ModuleStatus.Unknown;
    public override IStatusMessage GetStatusMessage() => new StatusMessage
    {
        Message = "Butts",
        SourceModule = ModuleName
    };
}