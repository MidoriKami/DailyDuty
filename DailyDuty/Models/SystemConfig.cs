namespace DailyDuty.Models;

public class SystemConfig
{
    public string CharacterName { get; set; } = string.Empty;
    public string CharacterWorld { get; set; } = string.Empty;

    public bool HideDisabledModules = false;
}