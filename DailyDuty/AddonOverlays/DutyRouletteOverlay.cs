using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.AddonOverlays;

internal class DutyRouletteOverlay : IDisposable
{
    private record DutyFinderSearchResult(string SearchKey, uint TerritoryType);

    private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();

    private bool defaultColorSaved;

    private ByteColor userDefaultTextColor;

    private static DutyRouletteSettings RouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    private IEnumerable<TrackedRoulette> DutyRoulettes => RouletteSettings.TrackedRoulettes;

    private bool Enabled => RouletteSettings.Enabled.Value && RouletteSettings.OverlayEnabled.Value;

    public DutyRouletteOverlay()
    {
        DutyFinderAddon.Instance.Refresh += OnRefresh;
        DutyFinderAddon.Instance.Draw += OnDraw;
        DutyFinderAddon.Instance.Finalize += OnFinalize;

        var rouletteData = LuminaCache<ContentRoulette>.Instance.GetAll()
            .Where(cr => cr.Name != string.Empty);

        foreach (var cr in rouletteData)
        {
            var simplifiedString = Regex.Replace(cr.Category.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

            dutyRouletteDuties.Add(new DutyFinderSearchResult(simplifiedString, cr.RowId));
        }
    }

    public void Dispose()
    {
        DutyFinderAddon.Instance.Refresh -= OnRefresh;
        DutyFinderAddon.Instance.Draw -= OnDraw;
        DutyFinderAddon.Instance.Finalize -= OnFinalize;
        
        DutyFinderAddon.Instance.ResetLabelColors(userDefaultTextColor);
    }
 
    private void OnDraw(object? sender, IntPtr e)
    {
        if (defaultColorSaved == false)
        {
            var tree = DutyFinderAddon.Instance.GetBaseTreeNode();
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

    private void OnRefresh(object? sender, IntPtr e)
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

    private void OnFinalize(object? sender, IntPtr e) => ResetDefaultTextColor();

    private void ResetDefaultTextColor() => DutyFinderAddon.Instance.GetBaseTreeNode().SetColorAll(userDefaultTextColor);

    private void SetRouletteColors()
    {
        var treeNode = DutyFinderAddon.Instance.GetBaseTreeNode();

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

    private static bool IsTabSelected(uint tab)
    {
        var tabBar = DutyFinderAddon.Instance.GetTabBar();
        return tab == tabBar.GetSelectedTabIndex();
    }
}