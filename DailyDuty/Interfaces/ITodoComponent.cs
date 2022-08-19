using DailyDuty.Configuration.Components;
using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface ITodoComponent
{
    IModule ParentModule { get; }

    Setting<bool> Enabled { get; }

    CompletionType CompletionType { get; }
}