using System;
using DailyDuty.DataModels;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Misc;

namespace DailyDuty.Interfaces;

public abstract class AbstractModule : 
    IModule, 
    ILogicComponent, 
    IConfigurationComponent, 
    IStatusComponent,
    ITodoComponent,
    ITimerComponent
{
    public IConfigurationComponent ConfigurationComponent => this;
    public IStatusComponent StatusComponent => this;
    public ILogicComponent LogicComponent => this;
    public ITodoComponent TodoComponent => this;
    public ITimerComponent TimerComponent => this;
    
    public abstract GenericSettings GenericSettings { get; }

    // Common
    //
    public abstract ModuleName Name { get; }
    public IModule ParentModule => this;
    public virtual void Dispose() { }


    // Logic Component
    //
    public abstract ModuleStatus GetModuleStatus();
    public virtual string GetStatusMessage() => string.Empty;
    public virtual DalamudLinkPayload? DalamudLinkPayload => null;
    public virtual bool LinkPayloadActive => false;
    public virtual DateTime GetNextReset() => CompletionType switch
    {
        CompletionType.Daily => Time.NextDailyReset(),
        CompletionType.Weekly => Time.NextWeeklyReset(),
        _ => DateTime.MinValue
    };
    public virtual void DoReset() { }
    
    
    // Configuration Component
    //
    ISelectable IConfigurationComponent.Selectable => new ConfigurationSelectable(this);
    void IConfigurationComponent.DrawConfiguration() => DrawConfiguration();
    protected virtual void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    
    // Status Component
    //
    ISelectable IStatusComponent.Selectable => new StatusSelectable(this);
    void IStatusComponent.DrawStatus() => DrawStatus();
    protected virtual void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }

    
    // TodoComponent
    //
    public abstract CompletionType CompletionType { get; }
    public virtual bool HasLongLabel => false;
    protected virtual string TodoTaskLabel => Name.GetTranslatedString();
    public virtual string GetShortTaskLabel() => TodoTaskLabel;
    public virtual string GetLongTaskLabel() => TodoTaskLabel;
    

    // Timer Component
    //
    public virtual TimeSpan GetTimerPeriod() => CompletionType switch
    {
        CompletionType.Daily => TimeSpan.FromDays(1),
        CompletionType.Weekly => TimeSpan.FromDays(7),
        _ => TimeSpan.Zero
    };
}