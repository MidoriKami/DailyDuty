using System;
using System.Numerics;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Timers;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Timers
{
    internal class TimersWindow : Window, IDisposable, IWindow
    {
        private readonly CountdownTimers countdownTimers;
        private int frameCounter;

        public new WindowName WindowName => WindowName.Timers;

        private TimersWindowSettings Settings => Service.Configuration.Windows.Timers;

        private const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                      ImGuiWindowFlags.NoTitleBar |
                                                      ImGuiWindowFlags.NoScrollbar |
                                                      ImGuiWindowFlags.NoCollapse;

        private const ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                           ImGuiWindowFlags.NoDecoration |
                                                           ImGuiWindowFlags.NoInputs;

        public TimersWindow() : base("DailyDuty Timers")
        {
            Service.WindowSystem.AddWindow(this);

            Service.Framework.Update += Update;

            var timersList = Service.TimerManager.GetTimers(WindowName.Timers);
            countdownTimers = new CountdownTimers(timersList);
        }

        private void Update(Framework framework)
        {
            if (Service.LoggedIn == false)
            {
                IsOpen = false;
                return;
            }

            if(frameCounter++ % 10 != 0) return;

            bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];

            IsOpen = Settings.Open && !isInQuestEvent;

            if (Settings.HideInDuty == true)
            {
                if (Utilities.Condition.IsBoundByDuty() == true)
                {
                    IsOpen = false;
                }
            }

            Flags = Settings.ClickThrough ? ClickThroughFlags : DefaultFlags;
        }

        public override void PreDraw()
        {
            var clr = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(clr.X, clr.Y, clr.Z, Settings.Opacity));

            clr = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(clr.X, clr.Y, clr.Z, Settings.Opacity));

            ImGui.PushStyleColor(ImGuiCol.ResizeGrip, Vector4.Zero);
        }

        public override void Draw()
        {
            countdownTimers.Draw();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(3);
        }

        public void Dispose()
        {
            Service.Framework.Update -= Update;
            
            Service.WindowSystem.RemoveWindow(this);
        }
    }
}
