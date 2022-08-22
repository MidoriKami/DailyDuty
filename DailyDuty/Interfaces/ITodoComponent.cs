using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface ITodoComponent
{
    IModule ParentModule { get; }
    CompletionType CompletionType { get; }
    bool HasLongLabel { get; }
    string GetShortTaskLabel();
    string GetLongTaskLabel();
}