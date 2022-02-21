using System;
using DailyDuty.Data.Enums;

namespace DailyDuty.Interfaces
{
    public interface IWindow : IDisposable
    {
        public WindowName WindowName { get; }

    }
}
