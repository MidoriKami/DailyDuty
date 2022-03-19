﻿using System.Linq;
using System.Numerics;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Timers;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Timers
{
    internal class TimersWindow : Window, IWindow
    {
        private readonly CountdownTimers countdownTimers;

        public new WindowName WindowName => WindowName.Timers;

        private TimersWindowSettings Settings => Service.Configuration.Windows.Timers;

        public TimersWindow() : base("DailyDuty Timers")
        {
            Service.WindowSystem.AddWindow(this);

            var timersList = Service.TimerManager.GetTimers(WindowName.Timers);
            countdownTimers = new CountdownTimers(timersList);
        }

        public override void PreOpenCheck()
        {
            bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];
            int enabledTimers = countdownTimers.GetEnabledTimers().Length;

            IsOpen = !isInQuestEvent && Service.LoggedIn && Settings.Open && enabledTimers > 0;

            if (Settings.HideInDuty == true)
            {
                if (Utilities.Condition.IsBoundByDuty() == true)
                {
                    IsOpen = false;
                }
            }
        }

        public override void PreDraw()
        {
            var color = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(color.X, color.Y, color.Z, Settings.Opacity));

            color = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(color.X, color.Y, color.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            if (IsOpen == false) return;

            Flags = DrawFlags.DefaultFlags;

            Flags |= Settings.ClickThrough ? DrawFlags.LockPosition : ImGuiWindowFlags.None;

            countdownTimers.Draw();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(2);
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }
    }
}
