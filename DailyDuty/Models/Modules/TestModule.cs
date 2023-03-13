using System;
using DailyDuty.Abstracts;
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
    public override DateTime GetNextReset() => DateTime.UtcNow + TimeSpan.FromMinutes(1);
    public override TimeSpan GetResetPeriod() => TimeSpan.FromMinutes(1);
    protected override ModuleStatus GetModuleStatus() => ModuleStatus.Unknown;
    public override StatusMessage GetStatusMessage() => new()
    {
        Message = "Butts",
    };
}