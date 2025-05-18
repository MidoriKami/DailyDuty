using KamiLib.Configuration;

namespace DailyDuty.Models;

public class TodoConfig {
    public bool Enabled = false;
    
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;

    public static TodoConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "TodoList.config.json", () => new TodoConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "TodoList.config.json", this);
}