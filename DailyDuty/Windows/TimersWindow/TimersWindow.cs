using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Data.Components;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Timers;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.TimersWindow
{
    internal class TimersWindow : Window, IDisposable, ICommand
    {
        private readonly List<CountdownTimer> countdownTimers;

        private TimersWindowSettings Settings => Service.SystemConfiguration.Windows.Timers;

        public TimersWindow() : base("DailyDuty Timers")
        {
            Service.WindowSystem.AddWindow(this);

            countdownTimers = Service.TimerManager.Timers;
        }

        public override void PreOpenCheck()
        {
            if (!Service.LoggedIn || Service.ClientState.IsPvP)
            {
                IsOpen = false;
            }
            else if (Service.Condition[ConditionFlag.OccupiedInQuestEvent] || (Utilities.Condition.IsBoundByDuty() && Settings.HideInDuty))
            {
                IsOpen = false;
            }
            else
            {
                IsOpen = Settings.Enabled;
            }
        }

        public override void PreDraw()
        {
            var backgroundColor = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, Settings.Opacity));

            var borderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(borderColor.X, borderColor.Y, borderColor.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            Flags = DrawFlags.DefaultFlags;

            Flags |= Settings.ClickThrough ? ImGuiWindowFlags.NoInputs : ImGuiWindowFlags.None;

            if (countdownTimers.Count(t => t.TimerSettings.Enabled) > 0)
            {
                DrawTimers();
            }
            else
            {
                ImGui.TextWrapped(Strings.Features.TimersWindowEnableTimersWarning);
            }
        }

        private void DrawTimers()
        {
            var timers = countdownTimers.Where(x => x.TimerSettings.Enabled).ToArray();
            var totalWidth = (int)ImGui.GetContentRegionAvail().X;
            var spacing = (int)ImGui.GetStyle().ItemSpacing.X;
            var i = 0;

            while(i < timers.Length)
            {
                var width = 0;
                var count = 0;
                var resizeCount = 0;

                for(var j = i; j < timers.Length; j++)
                {
                    var timer = timers[j];
                    var w = timer.TimerSettings.TimerStyle.Size + (count > 0 ? spacing : 0);
                    if(count > 0 && width + w > totalWidth)
                        break;

                    count++;
                    width += w;
                    if(timer.TimerSettings.TimerStyle.StretchToFit)
                        resizeCount++;
                }

                var add = resizeCount > 0 ? ((totalWidth - width) / resizeCount) : 0;

                for(var j = i; j < i + count; j++)
                {
                    var timer = timers[j];

                    if(timer.TimerSettings.TimerStyle.StretchToFit)
                    {
                        timer.TimerSettings.TimerStyle.Size += add;
                        timer.DrawStyled();
                        timer.TimerSettings.TimerStyle.Size -= add;
                    }
                    else
                    {
                        timer.DrawStyled();
                    }

                    if(j < i + count - 1)
                        ImGui.SameLine();
                }

                i += count;
            }
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(2);
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == Strings.Command.TimersCommand)
            {
                if (ICommand.OpenCommand(secondaryCommand))
                    Settings.Enabled = true;

                if (ICommand.CloseCommand(secondaryCommand))
                    Settings.Enabled  = false;

                if (ICommand.ToggleCommand(secondaryCommand))
                    Settings.Enabled  = !Settings.Enabled ;

                if (ICommand.HelpCommand(secondaryCommand))
                {
                    Chat.Print(Strings.Features.TimersWindowLabel, Strings.Command.TimersShowHelp);
                }

                Service.SystemConfiguration.Save();
            }
        }
    }
}
