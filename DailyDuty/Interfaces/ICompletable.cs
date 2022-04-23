using DailyDuty.Enums;

namespace DailyDuty.Interfaces
{
    internal interface ICompletable
    {
        public CompletionType Type { get; }
        public bool IsCompleted();
    }
}