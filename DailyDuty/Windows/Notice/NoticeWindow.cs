using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Notice;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Notice
{
    internal class NoticeWindow : Window, IWindow
    {
        private const uint CurrentVersion = 2;
        private NoticeSettings Settings => Service.Configuration.Windows.Notice;
        public new WindowName WindowName => WindowName.Notice;
        private readonly TimedCloseButton closeButton;

        private readonly WindowSizeConstraints windowSizeConstraints = new()
        {
            MaximumSize = new Vector2(500, 250),
            MinimumSize = new Vector2(500, 250)

        };

        public NoticeWindow() : base("DailyDuty NoticeWindow")
        {
            SizeConstraints = windowSizeConstraints;

            Service.Framework.Update += FrameworkUpdate;
            Service.WindowSystem.AddWindow(this);

            closeButton = new(CloseWindowAction);
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;
            Service.WindowSystem.RemoveWindow(this);
        }

        private void FrameworkUpdate(Framework framework)
        {
            if (Service.LoggedIn == true)
            {
                if (Settings.Version != CurrentVersion)
                {
                    Settings.NoticeShown = false;
                    Settings.Version = CurrentVersion;
                    Service.Configuration.Save();
                }

                if (Settings.NoticeShown == false)
                {
                    IsOpen = true;
                }
            }
            else
            {
                IsOpen = false;
                closeButton.Reset();
            }
        }

        public override void PreDraw()
        {
            Flags = DrawFlags.AutoResize;
            
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1.0f));
        }

        public override void Draw()
        {
            ImGui.Spacing();

            ImGui.Text("DailyDuty is very much still a work in progress\n" +
                       "This is a HUGE project that I am working on in my free time when inspiration strikes");
            ImGui.Spacing();

            ImGui.TextColored(new Vector4(0.8f, 0.2f, 0.2f, 1.0f),
                "DailyDuty can only track things that happened AFTER the plugin was installed");

            ImGui.Spacing();

            ImGui.Text("Some Updates WILL reset various parts of the configuration file.\n" +
                       "You can use 'Temporary Edit Mode' in the settings window to correct errors");
            
            ImGui.Spacing();

            ImGui.Text("If you are having an issue where counts keep resetting after relaunching the game,\n" +
                       "You need to delete the 'DailyDuty.json' config file and relaunch the game\n" +
                       "Use 'Temporary Edit Mode' to manually correct each module");

            closeButton.Draw();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor();
        }

        private void CloseWindowAction()
        {
            IsOpen = false;

            Settings.NoticeShown = true;

            Service.Configuration.Save();

            Service.Framework.Update -= FrameworkUpdate;
        }

        public void Reset()
        {
            Settings.NoticeShown = false;

            closeButton.Reset();

            Service.Framework.Update += FrameworkUpdate;
        }
    }
}
