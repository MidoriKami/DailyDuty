using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.DailyDutyWindow
{
    internal class DailyDutyWindow : Window, ICommand, IWindow
    {
        private readonly SelectionPane selectionPane = new()
        {
            Padding = 4.0f,
            ScreenRatio = 0.5f,
        };

        public DailyDutyWindow() : base("###DailyDutyMainWindow")
        {
            Service.WindowSystem.AddWindow(this);

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(700, 400),
                MaximumSize = new(9999,9999)
            };

            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);

            selectionPane.Dispose();
        }

        public override void PreOpenCheck()
        {
            if (Service.SystemConfiguration.DeveloperMode)
                IsOpen = true;

            if (Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void PreDraw()
        {
            if (!IsOpen) return;

            WindowName = Strings.Configuration.DailyDutySettingsLabel + " - " + Service.CharacterConfiguration.CharacterName + "###DailyDutyMainWindow";
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            selectionPane.Draw();

        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == null)
            {
                Toggle();
            }
        }

        WindowName IWindow.WindowName => Enums.WindowName.Main;
    }
}
