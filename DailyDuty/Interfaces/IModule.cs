using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface IModule
{
    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    IStatusComponent StatusComponent { get; }
    ILogicComponent LogicComponent { get; }
}