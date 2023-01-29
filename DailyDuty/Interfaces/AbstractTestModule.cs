using System;
using DailyDuty.DataModels;
using DailyDuty.UserInterface.Components;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Configuration;
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public abstract class AbstractTestModule : IModule
{
    public ModuleName Name => ModuleName.TestModule;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }
    
    public GenericSettings GenericSettings { get; } = new()
    {
        Enabled = new Setting<bool>(true),
        NextReset = DateTime.UtcNow + TimeSpan.FromDays(1),
        Suppressed = new Setting<bool>(false),
        TimerTaskEnabled = new Setting<bool>(false),
        TodoTaskEnabled = new Setting<bool>(false),
        NotifyOnLogin = new Setting<bool>(false),
        NotifyOnZoneChange = new Setting<bool>(false),
        TodoUseLongLabel = new Setting<bool>(false),
        TimerSettings = new TimerSettings(),
    };

    protected AbstractTestModule()
    {
        ConfigurationComponent = new TestModuleConfigurationComponent(this);
        StatusComponent = new TestModuleStatusComponent(this);
        LogicComponent = new TestModuleLogicComponent(this);
        TodoComponent = new TestModuleTodoComponent(this);
        TimerComponent = new TestModuleTimerComponent(this);
    }
    
    public void Dispose() { }

    private class TestModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public ModuleStatus GetModuleStatus() => ModuleStatus.Unknown;
        public string GetStatusMessage() => "StatusMessage";
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;
        public DateTime GetNextReset() => DateTime.UtcNow + TimeSpan.FromDays(1);
        
        public TestModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }
        
        public void Dispose()
        {
            
        }

        public void DoReset()
        {
            // Do Nothing
        }
    }

    private class TestModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public TestModuleConfigurationComponent(IModule parentModule) => ParentModule = parentModule;
        
        public void Draw()
        {
            
        }
    }

    private class TestModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);
        public TestModuleStatusComponent(IModule parentModule) => ParentModule = parentModule;
        
        public void Draw()
        {
            
        }
    }
    
    private class TestModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => false;
        public TestModuleTodoComponent(IModule parentModule) => ParentModule = parentModule;
        public string GetShortTaskLabel() => "ShortTaskLabel";
        public string GetLongTaskLabel() => "LongTaskLabel";
    }

    private class TestModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public TestModuleTimerComponent(IModule parentModule) => ParentModule = parentModule;

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => DateTime.UtcNow + TimeSpan.FromDays(1);
    }
}