using DailyDuty.Abstracts;

namespace DailyDuty.Models.Settings;

public class CharacterSettings : ConfigurationVersion
{
    public override int Version { get; set; } = 4;

    public SystemSettings SystemSettings = new();
}