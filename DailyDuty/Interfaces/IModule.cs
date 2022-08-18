namespace DailyDuty.Interfaces;

internal interface IModule
{
    IConfigurationComponent ConfigurationComponent { get; }
    IStatusComponent StatusComponent { get; }
    ILogicComponent LogicComponent { get; }
}