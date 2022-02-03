using System;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;

namespace DailyDuty.System
{
    public abstract class Module : IDisposable
    {
        private int zoneChangeCounter = 0;

        protected Module()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += PreOnTerritoryChanged;
        }

        public abstract void Update();

        public virtual void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= PreOnTerritoryChanged;
        }

        protected virtual void OnLogin(object? sender, EventArgs e)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(task => OnLoginDelayed());
        }

        protected virtual void PreOnTerritoryChanged(object? sender, ushort e)
        {
            if (Service.LoggedIn == false) return;

            AlwaysOnTerritoryChanged(sender, e);

            zoneChangeCounter++;
            if (zoneChangeCounter % Service.Configuration.TerritoryUpdateStaggerRate != 0) return;

            ThrottledOnTerritoryChanged(sender, e);
        }

        protected virtual void AlwaysOnTerritoryChanged(object? sender, ushort e)
        {

        }

        protected abstract void OnLoginDelayed();
        protected abstract void ThrottledOnTerritoryChanged(object? sender, ushort e);

        public abstract bool IsCompleted();

        public abstract void DoDailyReset(Configuration.CharacterSettings settings);

        public abstract void DoWeeklyReset(Configuration.CharacterSettings settings);
    }
}
