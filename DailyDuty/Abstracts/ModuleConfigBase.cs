using DailyDuty.Models.Settings;
using KamiLib.Configuration;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    public Setting<bool> ModuleEnabled = new(false);
    public MessageSettings MessageSettings = new();
}