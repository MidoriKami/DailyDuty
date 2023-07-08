using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Views.Tabs;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.Interfaces;
using KamiLib.Utilities;
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
        
        CommandController.RegisterCommands(this);
    }
    
    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;
    protected override IEnumerable<ITabItem> GetRegularTabs() => regularTabs;

    public override bool DrawConditions()
    {
        if (Service.ClientState.IsPvP) return false;
        if (!Service.ClientState.IsLoggedIn) return false;

        return true;
    }

    protected override void DrawWindowExtras()
    {
        base.DrawWindowExtras();
        PluginVersion.Instance.DrawVersionText();
    }

    [BaseCommandHandler("OpenConfigWindow")]
    // ReSharper disable once UnusedMember.Local
    private void OpenConfigWindow()
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            return;
        }
            
        Toggle();
    }
}