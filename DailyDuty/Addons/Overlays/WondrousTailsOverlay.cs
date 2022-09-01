using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons.DataModels;
using DailyDuty.Addons.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.DataStructures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Addons.Overlays;

internal unsafe class WondrousTailsOverlay : IDisposable
{
    private record DutyFinderSearchResult(string SearchKey, uint Value);

    private readonly WondrousTailsBook wondrousTailsBook = new();

    private readonly List<DutyFinderSearchResult> contentFinderDuties = new();

    private IEnumerable<WondrousTailsTask> wondrousTailsStatus;

    private DutyRouletteSettings DutyRouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;

    private bool Enabled => DutyRouletteSettings.Enabled.Value && DutyRouletteSettings.OverlayEnabled.Value;

    public WondrousTailsOverlay()
    {
        Service.AddonManager[AddonName.DutyFinder].OnRefresh += DutyFinderRefresh;
        Service.AddonManager[AddonName.DutyFinder].OnUpdate += DutyFinderUpdate;
        Service.AddonManager[AddonName.DutyFinder].OnDraw += DutyFinderDraw;
        Service.AddonManager[AddonName.DutyFinder].OnFinalize += DutyFinderFinalize;

        var contentFinderData = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
            !.Where(cfc => cfc.Name != string.Empty);

        foreach (var cfc in contentFinderData)
        {
            var simplifiedString = Regex.Replace(cfc.Name.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

            contentFinderDuties.Add(new DutyFinderSearchResult(simplifiedString, cfc.TerritoryType.Row));
        }

        wondrousTailsStatus = wondrousTailsBook.GetAllTaskData();
    }

    public void Dispose()
    {
        Service.AddonManager.GetAddonByType<DutyFinderAddon>().HideCloverNodes();

        Service.AddonManager[AddonName.DutyFinder].OnRefresh -= DutyFinderRefresh;
        Service.AddonManager[AddonName.DutyFinder].OnUpdate -= DutyFinderUpdate;
        Service.AddonManager[AddonName.DutyFinder].OnDraw -= DutyFinderDraw;
        Service.AddonManager[AddonName.DutyFinder].OnFinalize -= DutyFinderFinalize;
    }

    private void DutyFinderRefresh(object? sender, IntPtr e)
    {
        if (Enabled)
        {
            wondrousTailsStatus = wondrousTailsBook.GetAllTaskData();
        }
    }
    
    private void DutyFinderUpdate(object? sender, IntPtr e)
    {
        if (Enabled)
        {
            UpdateWondrousTails(e.ToAtkUnitBase());
        }
    }

    private void DutyFinderDraw(object? sender, IntPtr e)
    {
        if (Enabled)
        {
            var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();
            var treeNode = addon.GetBaseTreeNode(e.ToAtkUnitBase());

            treeNode.MakeCloverNodes();
        }
    }

    private void DutyFinderFinalize(object? sender, IntPtr e)
    {
        Service.AddonManager.GetAddonByType<DutyFinderAddon>().HideCloverNodes();
    }

    private ButtonState? IsWondrousTailsDuty(DutyFinderTreeListItem item)
    {
        var nodeString = item.Label;
        var nodeRegexString = item.FilteredLabel;
        
        var containsEllipsis = nodeString.Contains("...");

        foreach (var result in contentFinderDuties)
        {
            if (containsEllipsis)
            {
                var nodeStringLength = nodeRegexString.Length;

                if (result.SearchKey.Length <= nodeStringLength) continue;

                if (result.SearchKey[..nodeStringLength] == nodeRegexString)
                {
                    return InWondrousTailsBook(result.Value);
                }
            }
            else if (result.SearchKey == nodeRegexString)
            {
                return InWondrousTailsBook(result.Value);
            }
        }

        return null;
    }

    private ButtonState? InWondrousTailsBook(uint duty)
    {
        return wondrousTailsStatus.FirstOrDefault(task => task.DutyList.Contains(duty))?.TaskState;
    }

    private void UpdateWondrousTails(AtkUnitBase* baseNode)
    {
        var addon = Service.AddonManager.GetAddonByType<DutyFinderAddon>();
        var treeNode = addon.GetBaseTreeNode(baseNode);

        foreach (var item in treeNode.Items)
        {
            var taskState = IsWondrousTailsDuty(item);

            if (taskState == null || !wondrousTailsBook.PlayerHasBook())
            {
                item.CloverNode.SetVisibility(CloverState.Hidden);
            }
            else if (taskState == ButtonState.Unavailable)
            {
                item.CloverNode.SetVisibility(CloverState.Dark);
            }
            else if (taskState is ButtonState.AvailableNow or ButtonState.Completable)
            {
                item.CloverNode.SetVisibility(CloverState.Golden);
            }
        }
    }
}