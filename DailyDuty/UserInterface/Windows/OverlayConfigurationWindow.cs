using System.Collections.Generic;
using System.Numerics;
using DailyDuty.UserInterface.Components;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Windows;

namespace DailyDuty.UserInterface.Windows;

public class OverlayConfigurationWindow : SelectionWindow
{
    private readonly List<ISelectable> selectables = new()
    {
        new TimersConfigurationSelectable(),
        new TodoConfigurationSelectable()
    };
    
    public OverlayConfigurationWindow() : base("Overlay Configuration Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 450),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }

    protected override IEnumerable<ISelectable> GetSelectables() => selectables;
}