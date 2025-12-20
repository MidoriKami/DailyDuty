using KamiLib.Configuration;

namespace DailyDuty.Configs;

public class TodoConfig {
    public bool Enabled = false;
    
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;

    public static TodoConfig Load() 
        => Service.PluginInterface.LoadCharacterFile<TodoConfig>(Service.PlayerState.ContentId, "TodoList.config.json");

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.PlayerState.ContentId, "TodoList.config.json", this);
}