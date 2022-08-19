using DailyDuty.Configuration.Components;
using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface IModule
{
    GenericSettings GenericSettings { get; }
    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    IStatusComponent StatusComponent { get; }
    ILogicComponent LogicComponent { get; }
    ITodoComponent TodoComponent { get; }
}