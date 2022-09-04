using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

internal class ChallengeLogSettings : GenericSettings
{
    public int Commendations = 0;
    public int RouletteDungeons = 0;
    public int DungeonMaster = 0;

    public Setting<bool> CommendationWarning = new(true);
    public Setting<bool> RouletteDungeonWarning = new(true);
    public Setting<bool> DungeonWarning = new(true);
}