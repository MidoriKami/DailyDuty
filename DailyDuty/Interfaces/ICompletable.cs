using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces;

internal interface ICompletable
{
    public CompletionType Type { get; }
    public string HeaderText { get; }
    public GenericSettings GenericSettings { get; }

    public bool IsCompleted();
}