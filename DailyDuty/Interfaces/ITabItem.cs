using System;

namespace DailyDuty.Interfaces
{
    internal interface ITabItem : IDisposable
    {
        string TabName { get; }

        void Draw();

    }
}