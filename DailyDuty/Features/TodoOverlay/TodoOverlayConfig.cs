using DailyDuty.Classes;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlayConfig : ModuleConfig<TodoOverlayConfig> {
    protected override string FileName => "TodoList";
    
    public bool Enabled = false;
    
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;
}
