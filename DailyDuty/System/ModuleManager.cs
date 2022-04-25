using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Modules;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.System
{
    public class ModuleManager : IDisposable
    {
        private readonly List<object> modules = new()
        {
            // Daily
            new DutyRouletteModule(),
            new WondrousTailsModule(),
            new TreasureMapModule(),

        };

        private readonly List<IZoneChangeLogic> zoneChangeLogicModules;
        private readonly List<IZoneChangeThrottledNotification> zoneChangeThrottledNotificationModules;
        private readonly List<IZoneChangeAlwaysNotification> zoneChangeAlwaysNotificationModules;
        private readonly List<ILoginNotification> loginNotificationModules;
        private readonly List<IResettable> resettableModules;
        private readonly List<ICommand> commandModules;
        private readonly List<IChatHandler> chatHandlers;

        private readonly Queue<IUpdateable> updateQueue;
        private readonly Stopwatch resetDelayStopwatch = new();

        private readonly Stopwatch reminderThrottleStopwatch = new();

        public ModuleManager()
        {
            updateQueue = new(modules.OfType<IUpdateable>());
            zoneChangeLogicModules = modules.OfType<IZoneChangeLogic>().ToList();
            zoneChangeThrottledNotificationModules = modules.OfType<IZoneChangeThrottledNotification>().ToList();
            zoneChangeAlwaysNotificationModules = modules.OfType<IZoneChangeAlwaysNotification>().ToList();
            loginNotificationModules = modules.OfType<ILoginNotification>().ToList();
            resettableModules = modules.OfType<IResettable>().ToList();
            commandModules = modules.OfType<ICommand>().ToList();
            chatHandlers = modules.OfType<IChatHandler>().ToList();

            Service.Framework.Update += Update;
            Service.ClientState.Login += PreOnLogin;
            Service.ClientState.TerritoryChanged += PreOnTerritoryChanged;
            Service.Chat.ChatMessage += OnChatMessage;
        }

        private void PreOnTerritoryChanged(object? sender, ushort e)
        {
            var timer = reminderThrottleStopwatch;
            var timerDelay = Service.SystemConfiguration.System.MinutesBetweenThrottledMessages;

            foreach (var module in zoneChangeLogicModules)
            {
                module.HandleZoneChange(sender, e);
            }

            if (Service.LoggedIn == false) return;

            AlwaysOnTerritoryChanged();

            if(timer.Elapsed.Minutes >= timerDelay || timer.IsRunning == false)
            {
                reminderThrottleStopwatch.Restart();
                ThrottledOnTerritoryChanged();
            }
        }

        private void ThrottledOnTerritoryChanged()
        {
            foreach (var module in zoneChangeThrottledNotificationModules)
            {
                module.TrySendNotification();
            }
        }

        private void AlwaysOnTerritoryChanged()
        {
            foreach (var module in zoneChangeAlwaysNotificationModules)
            {
                module.TrySendNotification();
            }
        }

        private void PreOnLogin(object? sender, EventArgs e)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => OnLoginDelayed());

            reminderThrottleStopwatch.Restart();
        }

        private void OnLoginDelayed()
        {
            foreach (var module in loginNotificationModules)
            {
                module.TrySendNotification();
            }
        }

        private void Update(Framework framework)
        {
            if (Service.LoggedIn == false) return;

            Time.UpdateDelayed(resetDelayStopwatch, TimeSpan.FromSeconds(1), UpdateResets);

            var module = updateQueue.Dequeue();

            module.Update();

            updateQueue.Enqueue(module);
        }

        private void UpdateResets()
        {
            foreach (var resettable in resettableModules)
            {
                if (!resettable.NeedsResetting()) continue;
                    
                resettable.DoReset();
            }
        }

        public void ProcessCommand(string command, string arguments)
        {
            foreach (var module in commandModules)
            {
                module.ProcessCommand(command, arguments);
            }
        }

        internal List<ICompletable> GetCompletables(CompletionType type)
        {
            var completableModules = modules
                .OfType<ICompletable>()
                .Where(module => module.Type == type)
                .ToList();

            return completableModules;
        }

        public T? GetModule<T>()
        {
            return modules.OfType<T>().FirstOrDefault();
        }

        private void OnChatMessage(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            foreach (var module in chatHandlers)
            {
                module.HandleChat(type, senderID, ref sender, ref message, ref isHandled);
            }
        }

        public void Dispose()
        {
            foreach (var module in modules.OfType<IDisposable>())
            {
                module.Dispose();
            }

            Service.Framework.Update -= Update;
            Service.ClientState.Login -= PreOnLogin;
            Service.ClientState.TerritoryChanged -= PreOnTerritoryChanged;
            Service.Chat.ChatMessage -= OnChatMessage;
        }
    }
}