using System;
using System.Collections.Generic;
using System.IO;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Newtonsoft.Json;

namespace DailyDuty.Data
{
    [Serializable]
    public class CharacterLogFile
    {
        public int Version { get; set; } = 1;

        public string CharacterName = "Unknown";
        public ulong LocalContentID = 0;
        public string World = "UnknownWorld";

        public Dictionary<ModuleType, List<LogMessage>> Messages = new();

        public void Save()
        {
            if (this is not {CharacterName: "Unknown", LocalContentID: 0, World: "UnknownWorld"})
            {
                var logFileInfo = Configuration.GetLogFileInfo(LocalContentID);

                File.WriteAllText(logFileInfo.FullName, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }
    }
}
