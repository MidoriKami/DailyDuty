using System;

namespace DailyDuty.System
{
    internal abstract class Module : IDisposable
    {
        public abstract void Update();

        public abstract void Dispose();

        public abstract bool ModuleIsCompleted();
    }
}
