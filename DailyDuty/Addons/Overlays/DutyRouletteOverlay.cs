using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons.DataModels;
using DailyDuty.Configuration.Components;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Addons.Overlays;

internal unsafe class DutyRouletteOverlay : IDisposable
{
    private record DutyFinderSearchResult(string SearchKey, uint TerritoryType);

    private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();

    private bool defaultColorSaved;

    private ByteColor userDefaultTextColor;

    private DutyRouletteSettings RouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    private IEnumerable<TrackedRoulette> DutyRoulettes => RouletteSettings.TrackedRoulettes;

    private bool Enabled => RouletteSettings.Enabled.Value && RouletteSettings.OverlayEnabled.Value;

    public DutyRouletteOverlay()
    {
        var dutyFinder = Service.AddonManager.Get<DutyFinderAddon>();

        dutyFinder.Refresh += OnRefresh;
        dutyFinder.Draw += OnDraw;
        dutyFinder.Finalize += OnFinalize;

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
        var dutyFinder = Service.AddonManager.Get<DutyFinderAddon>();

        dutyFinder.ResetLabelColors(userDefaultTextColor);

        dutyFinder.Refresh -= OnRefresh;
        dutyFinder.Draw -= OnDraw;
        dutyFinder.Finalize -= OnFinalize;
    }
 
    private void OnDraw(object? sender, IntPtr e)
    {
        if (defaultColorSaved == false)
        {
            var addon = Service.AddonManager.Get<DutyFinderAddon>();
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

    private void OnRefresh(object? sender, IntPtr e)
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

    private void OnFinalize(object? sender, IntPtr e)
    {
        ResetDefaultTextColor(e.ToAtkUnitBase());
    }

    private void ResetDefaultTextColor(AtkUnitBase* rootNode)
    {
        var addon = Service.AddonManager.Get<DutyFinderAddon>();

        addon.GetBaseTreeNode(rootNode).SetColorAll(userDefaultTextColor);
    }

    private void SetRouletteColors(AtkUnitBase* rootNode)
    {
        var addon = Service.AddonManager.Get<DutyFinderAddon>();
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
        var addon = Service.AddonManager.Get<DutyFinderAddon>();
        var tabBar = addon.GetTabBar(addonBase);

        return tab == tabBar.GetSelectedTabIndex();
    }
}