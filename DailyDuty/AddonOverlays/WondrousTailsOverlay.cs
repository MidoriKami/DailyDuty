using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.Addons.ContentsFinder;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Modules;

namespace DailyDuty.AddonOverlays;

internal class WondrousTailsOverlay : IDisposable
{
    private IEnumerable<WondrousTailsTask> wondrousTailsStatus;

    private static WondrousTailsSettings DutyRouletteSettings => Service.ConfigurationManager.CharacterConfiguration.WondrousTails;

    private static bool Enabled => DutyRouletteSettings is {Enabled.Value: true, OverlayEnabled.Value: true};

    public WondrousTailsOverlay()
    {
        AddonContentsFinder.Instance.Refresh += OnRefresh;
        AddonContentsFinder.Instance.Update += OnUpdate;
        AddonContentsFinder.Instance.Draw += OnDraw;
        AddonContentsFinder.Instance.Finalize += OnFinalize;
        
        wondrousTailsStatus = WondrousTailsBook.Instance.GetAllTaskData();
    }

    public void Dispose()
    {
        AddonContentsFinder.HideCloverNodes();

        AddonContentsFinder.Instance.Refresh -= OnRefresh;
        AddonContentsFinder.Instance.Update -= OnUpdate;
        AddonContentsFinder.Instance.Draw -= OnDraw;
        AddonContentsFinder.Instance.Finalize -= OnFinalize;
    }

    private void OnRefresh(object? sender, nint e)
    {
        if (!Enabled) return;

        wondrousTailsStatus = WondrousTailsBook.Instance.GetAllTaskData();
    }
    
    private void OnUpdate(object? sender, nint e)
    {
        UpdateWondrousTails();
    }

    private void OnDraw(object? sender, nint e)
    {
        if (!Enabled) return;

        var treeNode = AddonContentsFinder.GetBaseTreeNode();

        treeNode.MakeCloverNodes();
    }

    private void OnFinalize(object? sender, nint e) => AddonContentsFinder.HideCloverNodes();

    private ButtonState? IsWondrousTailsDuty(DutyFinderTreeListItem item)
    {
        var nodeString = item.Label;
        var nodeRegexString = item.FilteredLabel;
        
        var containsEllipsis = nodeString.Contains("...");

        foreach (var result in AddonContentsFinder.Instance.Duties)
        {
            if (containsEllipsis)
            {
                var nodeStringLength = nodeRegexString.Length;

                if (result.SearchKey.Length <= nodeStringLength) continue;

                if (result.SearchKey[..nodeStringLength] == nodeRegexString)
                {
                    return InWondrousTailsBook(result.TerritoryType);
                }
            }
            else if (result.SearchKey == nodeRegexString)
            {
                return InWondrousTailsBook(result.TerritoryType);
            }
        }

        return null;
    }

    private ButtonState? InWondrousTailsBook(uint duty) => wondrousTailsStatus.FirstOrDefault(task => task.DutyList.Contains(duty))?.TaskState;

    private void UpdateWondrousTails()
    {
        var treeNode = AddonContentsFinder.GetBaseTreeNode();

        foreach (var item in treeNode.Items)
        {
            var taskState = IsWondrousTailsDuty(item);

            if (taskState == null || !WondrousTailsBook.PlayerHasBook || !Enabled)
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