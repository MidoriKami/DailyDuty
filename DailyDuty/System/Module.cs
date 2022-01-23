using System;

namespace DailyDuty.System
{
    internal abstract class Module : IDisposable
    {
        public virtual void UpdateSlow()
        {

        }

        public virtual void Update()
        {
            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            UpdateSlow();
        }

        public abstract void Dispose();

        public abstract bool IsCompleted();

        public abstract void DoDailyReset();

        public abstract void DoWeeklyReset();
    }
}
