using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Modules;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.AddonOverlays;

internal class WondrousTailsOverlay : IDisposable
{
    private record DutyFinderSearchResult(string SearchKey, uint Value);

    private readonly WondrousTailsBook wondrousTailsBook = new();

    private readonly List<DutyFinderSearchResult> contentFinderDuties = new();

    private IEnumerable<WondrousTailsTask> wondrousTailsStatus;

    private static WondrousTailsSettings DutyRouletteSettings => Service.ConfigurationManager.CharacterConfiguration.WondrousTails;

    private static bool Enabled => DutyRouletteSettings.Enabled.Value && DutyRouletteSettings.OverlayEnabled.Value;

    public WondrousTailsOverlay()
    {
        DutyFinderAddon.Instance.Refresh += OnRefresh;
        DutyFinderAddon.Instance.Update += OnUpdate;
        DutyFinderAddon.Instance.Draw += OnDraw;
        DutyFinderAddon.Instance.Finalize += OnFinalize;
        
        var contentFinderData = LuminaCache<ContentFinderCondition>.Instance.GetAll()
            .Where(cfc => cfc.Name != string.Empty);

        foreach (var cfc in contentFinderData)
        {
            var simplifiedString = Regex.Replace(cfc.Name.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

            contentFinderDuties.Add(new DutyFinderSearchResult(simplifiedString, cfc.TerritoryType.Row));
        }

        wondrousTailsStatus = wondrousTailsBook.GetAllTaskData();
    }

    public void Dispose()
    {
        DutyFinderAddon.Instance.HideCloverNodes();

        DutyFinderAddon.Instance.Refresh -= OnRefresh;
        DutyFinderAddon.Instance.Update -= OnUpdate;
        DutyFinderAddon.Instance.Draw -= OnDraw;
        DutyFinderAddon.Instance.Finalize -= OnFinalize;
    }

    private void OnRefresh(object? sender, IntPtr e)
    {
        if (!Enabled) return;

        wondrousTailsStatus = wondrousTailsBook.GetAllTaskData();
    }
    
    private void OnUpdate(object? sender, IntPtr e)
    {
        UpdateWondrousTails();
    }

    private void OnDraw(object? sender, IntPtr e)
    {
        if (!Enabled) return;

        var treeNode = DutyFinderAddon.Instance.GetBaseTreeNode();

        treeNode.MakeCloverNodes();
    }

    private void OnFinalize(object? sender, IntPtr e) => DutyFinderAddon.Instance.HideCloverNodes();

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

    private ButtonState? InWondrousTailsBook(uint duty) => wondrousTailsStatus.FirstOrDefault(task => task.DutyList.Contains(duty))?.TaskState;

    private void UpdateWondrousTails()
    {
        var treeNode = DutyFinderAddon.Instance.GetBaseTreeNode();

        foreach (var item in treeNode.Items)
        {
            var taskState = IsWondrousTailsDuty(item);

            if (taskState == null || !wondrousTailsBook.PlayerHasBook() || !Enabled)
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