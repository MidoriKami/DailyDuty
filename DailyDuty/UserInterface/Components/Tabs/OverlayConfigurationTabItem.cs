using System.Collections.Generic;
using DailyDuty.Localization;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components.Tabs;

public class OverlayConfigurationTabItem : ISelectionWindowTab
{
    private readonly List<ISelectable> selectables = new()
    {
        new TimersConfigurationSelectable(),
        new TodoConfigurationSelectable()
    };

    public string TabName => Strings.Tabs_OverlayConfiguration;
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables() => selectables;
}