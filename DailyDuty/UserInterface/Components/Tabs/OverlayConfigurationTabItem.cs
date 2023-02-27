using System.Collections.Generic;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components.Tabs;

public class OverlayConfigurationTabItem : ISelectionWindowTab
{
    private readonly List<ISelectable> selectables = new()
    {
        new TimersConfigurationSelectable(),
        new TodoConfigurationSelectable()
    };

    public string TabName => "Overlay Configuration";
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables() => selectables;
}