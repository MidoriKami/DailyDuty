using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons.DataModels;
using DailyDuty.Addons.Enums;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.ModuleSettings;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Addons.Overlays;

internal unsafe class DutyFinderOverlay : IDisposable
{
    private record DutyFinderSearchResult(string SearchKey, uint TerritoryType);

    private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();

    private bool defaultColorSaved;

    private ByteColor userDefaultTextColor;

    private DutyRouletteSettings RouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    private IEnumerable<TrackedRoulette> DutyRoulettes => RouletteSettings.TrackedRoulettes;

    private bool Enabled => RouletteSettings.Enabled.Value && RouletteSettings.OverlayEnabled.Value;

    public DutyFinderOverlay()
    {
        Service.AddonManager[AddonName.DutyFinder].OnRefresh += DutyFinderRefresh;
        Service.AddonManager[AddonName.DutyFinder].OnDraw += DutyFinderDraw;
        Service.AddonManager[AddonName.DutyFinder].OnFinalize += DutyFinderFinalize;

        var rouletteData = Service.DataManager.GetExcelSheet<ContentRoulette>()
            !.Where(cr => cr.Name != string.Empty);

        foreach (var cr in rouletteData)
        {
            var simplifiedString = Regex.Replace(cr.Category.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

            dutyRouletteDuties.Add(new DutyFinderSearchResult(simplifiedString, cr.RowId));
        }
    }

    public void Dispose()
    {
        Service.AddonManager.GetAddonByType<DutyFinderAddon>().ResetLabelColors(userDefaultTextColor);

        Service.AddonManager[AddonName.DutyFinder].OnRefresh -= DutyFinderRefresh;
        Service.AddonManager[AddonName.DutyFinder].OnDraw -= DutyFinderDraw;
        Service.AddonManager[AddonName.DutyFinder].OnFinalize -= DutyFinderFinalize;
    }
 
    private void DutyFinderDraw(object? sender, IntPtr e)
    {
        if (defaultColorSaved == false)
        {
            var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();
            var tree = addon.GetBaseTreeNode(e.ToAtkUnitBase());
            var line = tree.Items.First();
            userDefaultTextColor = line.GetTextColor();
            defaultColorSaved = true;
        }

        if (Enabled)
        {
            if (IsTabSelected(0, e.ToAtkUnitBase()) == false)
                ResetDefaultTextColor(e.ToAtkUnitBase());
            else
                SetRouletteColors(e.ToAtkUnitBase());
        }
        else
        {
            ResetDefaultTextColor(e.ToAtkUnitBase());
        }
    }

    private void DutyFinderRefresh(object? sender, IntPtr e)
    {
        if (Enabled)
        {
            if (IsTabSelected(0, e.ToAtkUnitBase()) == false)
                ResetDefaultTextColor(e.ToAtkUnitBase());
            else
                SetRouletteColors(e.ToAtkUnitBase());
        }
        else
        {
            ResetDefaultTextColor(e.ToAtkUnitBase());
        }
    }

    private void DutyFinderFinalize(object? sender, IntPtr e)
    {
        ResetDefaultTextColor(e.ToAtkUnitBase());
    }

    private void ResetDefaultTextColor(AtkUnitBase* rootNode)
    {
        var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();

        addon.GetBaseTreeNode(rootNode).SetColorAll(userDefaultTextColor);
    }

    private void SetRouletteColors(AtkUnitBase* rootNode)
    {
        var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();
        var treeNode = addon.GetBaseTreeNode(rootNode);

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
        var dutyFinderResult = dutyRouletteDuties.FirstOrDefault(duty => duty.SearchKey == item.FilteredLabel);
        if (dutyFinderResult == null) return null;

        return DutyRoulettes.FirstOrDefault(duty => (uint) duty.Roulette == dutyFinderResult.TerritoryType);
    }

    private bool IsTabSelected(uint tab, AtkUnitBase* addonBase)
    {
        var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();
        var tabBar = addon.GetTabBar(addonBase);

        return tab == tabBar.GetSelectedTabIndex();
    }
}