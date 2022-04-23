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

        public void LogMessage(ModuleName module, string message)
        {
            if (!Log.Messages.ContainsKey(module))
            {
                Log.Messages.Add(module, new List<LogMessage>());
            }

            var moduleMessages = Log.Messages[module];
            var moduleMessagesCount = moduleMessages.Count;

            if (moduleMessagesCount > 30)
            {
                moduleMessages.RemoveAt(moduleMessagesCount - 1);
            }

            moduleMessages.Add(new LogMessage()
            {
                Message = message,
                ModuleName = module,
                Time = DateTime.UtcNow,
            });

            Save();
        }

        public IEnumerable<LogMessage> GetMessages(ModuleName moduleName)
        {
            if (!Log.Messages.ContainsKey(moduleName))
            {
                Log.Messages[moduleName] = new List<LogMessage>();
            }

            return Log.Messages[moduleName].OrderByDescending(m => m.Time);
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
