using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Views.Tabs;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.Views;

public class ConfigurationWindow : TabbedSelectionWindow
{
    private readonly List<ISelectionWindowTab> tabs;
    
    public ConfigurationWindow() : base("DailyDuty - Configuration Window")
    {
        tabs = new List<ISelectionWindowTab>
        {
            new ModuleConfigurationTab(),
            new ModuleDataTab(),
        };
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(600, 300),
            MaximumSize = new Vector2(9999,9999)
        };
    }
    
    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;
}