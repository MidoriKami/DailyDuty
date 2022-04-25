using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data;
using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.System
{
    public class LogManager : IDisposable
    {
        public CharacterLogFile Log { get; set; } = new();

        public void LogMessage(ModuleType module, string message)
        {
            if (!Log.Messages.ContainsKey(module))
            {
                Log.Messages.Add(module, new List<LogMessage>());
            }

            var moduleMessages = Log.Messages[module];
            var moduleMessagesCount = moduleMessages.Count;

            if (moduleMessagesCount > 30)
            {
                moduleMessages.RemoveAt(0);
            }

            moduleMessages.Add(new LogMessage()
            {
                Message = message,
                ModuleType = module,
                Time = DateTime.UtcNow,
            });

            Save();
        }

        public IEnumerable<LogMessage> GetMessages(ModuleType moduleType)
        {
            if (!Log.Messages.ContainsKey(moduleType))
            {
                Log.Messages[moduleType] = new List<LogMessage>();
            }

            return Log.Messages[moduleType].OrderByDescending(m => m.Time);
        }

        public void Save()
        {
            Log.Save();
        }

        public void Dispose()
        {
            Save();
        }
    }
}
