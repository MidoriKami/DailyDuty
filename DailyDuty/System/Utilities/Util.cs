using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.System.Utilities
{
    internal static class Util
    {
        public static unsafe bool IsTargetable(PartyMember partyMember)
        {
            var playerGameObject = partyMember.GameObject;
            if (playerGameObject == null) return false;

            var playerTargetable = ((GameObject*)playerGameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }

        public static unsafe bool IsTargetable(Dalamud.Game.ClientState.Objects.Types.GameObject gameObject)
        {
            var playerTargetable = ((GameObject*)gameObject.Address)->GetIsTargetable();

            return playerTargetable;
        }

        public static void PrintMessage(string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText("[DailyDuty] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        private static void PrintColoredMessage(string firstTag, string secondTag, string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[{firstTag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(62);
            stringBuilder.AddText($"[{secondTag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        private static void PrintColoredMessage(string firstTag, string secondTag, string thirdTag, string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[{firstTag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(62);
            stringBuilder.AddText($"[{secondTag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(523);
            stringBuilder.AddText($"[{thirdTag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        public static void PrintWondrousTails(string message)
        {
            PrintColoredMessage("DailyDuty", "WondrousTails", message);
        }

        public static void PrintTreasureMap(string message)
        {
            PrintColoredMessage("DailyDuty", "TreasureMap", message);
        }

        public static void PrintCustomDelivery(string message)
        {
            PrintColoredMessage("DailyDuty", "CustomDelivery", message);
        }

        public static void PrintMiniCactpot(string message)
        {
            PrintColoredMessage("DailyDuty", "MiniCactpot", message);
        }

        public static void PrintDebug(string message)
        {
            PrintColoredMessage("DailyDuty", "Debug", message);
        }

        public static void PrintDebug(string tag, string message)
        {
            PrintColoredMessage("DailyDuty", "Debug", tag, message);
        }

        // Run function immediately, and prevent re-execution for TimeSpan delay time
        public static void UpdateDelayed(Stopwatch stopwatch, TimeSpan delayTime, Action function)
        {
            if (stopwatch.IsRunning && stopwatch.Elapsed >= delayTime)
            {
                stopwatch.Stop();
                stopwatch.Reset();
            }

            if (stopwatch.IsRunning == false)
            {
                stopwatch.Start();
                function();
            }
        }

        public static DateTime NextDailyReset()
        {
            var now = DateTime.UtcNow;
            
            if( now.Hour < 15 )
            {
                return now.Date.AddHours(15);   
            }
            else
            {
                return now.AddDays(1).Date.AddHours(15);
            }
        }

        public static DateTime NextWeeklyReset()
        {
            var today = DateTime.UtcNow;
            
            if(today.Hour < 8 && today.DayOfWeek == DayOfWeek.Tuesday)
            {
                return today.Date.AddHours(8);
            }
            else
            {
                var nextReset = today.AddDays(1);

                while (nextReset.DayOfWeek != DayOfWeek.Tuesday)
                {
                    nextReset = nextReset.AddDays(1);
                }
                
                return nextReset.Date.AddHours(8);
            }
        }

        public static void DelayedLoginMessage(TimeSpan delayTime, string message, Action<string> formatter)
        {
            Task.Delay(delayTime).ContinueWith(t =>
            {
                formatter(message);
            });
        }

        public static void LogList<T>(IEnumerable<T> list)
        {
            PluginLog.Information(FormatList(list));
        }

        public static string FormatList<T>(IEnumerable<T> list)
        {
            return "{ " + string.Join(", ", list) + " }";
        }
    }
}
