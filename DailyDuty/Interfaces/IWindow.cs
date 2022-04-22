using System;
using DailyDuty.Enums;

namespace DailyDuty.Interfaces
{
    public interface IWindow : IDisposable
    {
        public WindowName WindowName { get; }

    }
}
