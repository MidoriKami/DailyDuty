using System.Collections.Generic;
using DailyDuty.Views.Tabs;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.Views;

public class ConfigurationWindow : TabbedSelectionWindow
{
    private readonly List<ISelectionWindowTab> tabs;
    
    public ConfigurationWindow() : base("Configuration Window")
    {
        tabs = new List<ISelectionWindowTab>
        {
            new ModuleConfigurationTab(),
            new ModuleDataTab(),
        };
    }
    
    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;
}