using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.TodoOverlay;

public class Config : ConfigBase {
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;

    public List<TodoPanelConfig> Panels = [];
}
