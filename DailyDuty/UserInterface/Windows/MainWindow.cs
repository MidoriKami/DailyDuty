using System;
using DailyDuty.Utilities;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;


namespace DailyDuty.UserInterface.Windows
{
    internal class MainWindow : Window, IDisposable
    {
        public MainWindow() : base("###DailyDutyMainWindow")
        {
            Log.Verbose("Constructing MainWindow");
        }

        public void Dispose()
        {

        }

        public override void PreOpenCheck()
        {
            if (Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void PreDraw()
        {

        }

        public override void Draw()
        {

        }

        public override void PostDraw()
        {

        }

        public override void OnClose()
        {
            Service.PluginInterface.UiBuilder.AddNotification("System Settings Saved", "DailyDuty", NotificationType.Success);
        }
    }
}
