using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface ILogicComponent
{
    IModule ParentModule { get; }

    ModuleStatus GetModuleStatus();
}