using System;
using System.Collections.Generic;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.Views;

public class ConfigurationWindow : TabbedSelectionWindow
{
    public ConfigurationWindow() : base("Configuration Window")
    {
        
    }
    
    protected override IEnumerable<ISelectionWindowTab> GetTabs() => Array.Empty<ISelectionWindowTab>();
}