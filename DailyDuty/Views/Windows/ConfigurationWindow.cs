using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Views.Tabs;
using KamiLib.Interfaces;
using KamiLib.Misc;
using KamiLib.Windows;

namespace DailyDuty.Views;

public class ConfigurationWindow : TabbedSelectionWindow
{
    private readonly List<ISelectionWindowTab> tabs;
    private readonly List<ITabItem> regularTabs;
    
    public ConfigurationWindow() : base("DailyDuty - Configuration Window", 55.0f)
    {
        tabs = new List<ISelectionWindowTab>
        {
            new ModuleConfigurationTab(),
            new ModuleDataTab(),
        };

        regularTabs = new List<ITabItem>
        {
            new TodoConfigurationTab(),
        };
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(650, 400),
            MaximumSize = new Vector2(9999,9999)
        };
    }
    
    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;
    protected override IEnumerable<ITabItem> GetRegularTabs() => regularTabs;

    protected override void DrawWindowExtras()
    {
        base.DrawWindowExtras();
        PluginVersion.Instance.DrawVersionText();
    }
}