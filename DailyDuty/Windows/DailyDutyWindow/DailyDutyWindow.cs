using System.Numerics;
using DailyDuty.Data;
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
            Padding = 6.0f,
            SelectionPaneWidth = 300.0f
        };

        private SettingsWindowSettings Settings => Service.SystemConfiguration.Windows.Settings;

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
        }

        public override void PreOpenCheck()
        {
            //if (Service.SystemConfiguration.DeveloperMode)
            //    IsOpen = true;

            if (Service.LoggedIn == false)
                IsOpen = false;

            if (Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void PreDraw()
        {
            if (!IsOpen) return;

            WindowName = Strings.Configuration.DailyDutySettingsLabel + " - " + Service.CharacterConfiguration.CharacterName + "###DailyDutyMainWindow";

            var backgroundColor = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, Settings.Opacity));

            var borderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(borderColor.X, borderColor.Y, borderColor.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            selectionPane.Draw();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(2);
        }

        // Todo: Add Popup notification on save
        public override void OnClose()
        {
            Service.SystemConfiguration.Save();
            Service.CharacterConfiguration.Save();
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
