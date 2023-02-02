using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.Addons.ContentsFinder;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Client.Graphics;

namespace DailyDuty.AddonOverlays;

internal class DutyRouletteOverlay : IDisposable
{
    private static DutyRouletteSettings RouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    private static IEnumerable<TrackedRoulette> DutyRoulettes => RouletteSettings.TrackedRoulettes;

    private static bool Enabled => RouletteSettings is {Enabled.Value: true, OverlayEnabled.Value: true};
    
    private bool defaultColorSaved;
    private ByteColor userDefaultTextColor;
    
    public DutyRouletteOverlay()
    {
        AddonContentsFinder.Instance.Refresh += OnRefresh;
        AddonContentsFinder.Instance.Draw += OnDraw;
        AddonContentsFinder.Instance.Finalize += OnFinalize;
    }

    public void Dispose()
    {
        AddonContentsFinder.Instance.Refresh -= OnRefresh;
        AddonContentsFinder.Instance.Draw -= OnDraw;
        AddonContentsFinder.Instance.Finalize -= OnFinalize;
        
        AddonContentsFinder.ResetLabelColors(userDefaultTextColor);
    }
 
    private void OnDraw(object? sender, nint e)
    {
        if (defaultColorSaved == false)
        {
            var tree = AddonContentsFinder.GetBaseTreeNode();
            var line = tree.Items.First();
            userDefaultTextColor = line.GetTextColor();
            defaultColorSaved = true;
        }

        if (Enabled)
        {
            if (IsTabSelected(0) == false)
                ResetDefaultTextColor();
            else
                SetRouletteColors();
        }
        else
        {
            ResetDefaultTextColor();
        }
    }

    private void OnRefresh(object? sender, nint e)
    {
        if (Enabled)
        {
            if (IsTabSelected(0) == false)
                ResetDefaultTextColor();
            else
                SetRouletteColors();
        }
        else
        {
            ResetDefaultTextColor();
        }
    }

    private void OnFinalize(object? sender, nint e) => ResetDefaultTextColor();

    private void ResetDefaultTextColor() => AddonContentsFinder.GetBaseTreeNode().SetColorAll(userDefaultTextColor);

    private void SetRouletteColors()
    {
        var treeNode = AddonContentsFinder.GetBaseTreeNode();

        foreach (var item in treeNode.Items)
        {
            if (IsRouletteDuty(item) is { } trackedRoulette)
            {
                switch (trackedRoulette)
                {
                    case { Tracked.Value: true, State: RouletteState.Complete }:
                        item.SetTextColor(RouletteSettings.CompleteColor.Value);
                        break;

                    case { Tracked.Value: true, State: RouletteState.Incomplete }:
                        item.SetTextColor(RouletteSettings.IncompleteColor.Value);
                        break;

                    case { Tracked.Value: true, State: RouletteState.Overriden }:
                        item.SetTextColor(RouletteSettings.OverrideColor.Value);
                        break;

                    default:
                        item.SetTextColor(userDefaultTextColor);
                        break;
                }
            }
        }
    }

    private TrackedRoulette? IsRouletteDuty(DutyFinderTreeListItem item)
    {
        var dutyFinderResult = AddonContentsFinder.Instance.Roulettes.FirstOrDefault(duty => duty.SearchKey == item.FilteredLabel);
        if (dutyFinderResult == null) return null;

        return DutyRoulettes.FirstOrDefault(duty => (uint) duty.Roulette == dutyFinderResult.TerritoryType);
    }

    private static bool IsTabSelected(uint tab)
    {
        var tabBar = AddonContentsFinder.GetTabBar();
        return tab == tabBar.GetSelectedTabIndex();
    }
}