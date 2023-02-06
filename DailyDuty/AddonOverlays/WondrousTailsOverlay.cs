using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.AddonOverlays;
public unsafe class WondrousTailsOverlay : IDisposable
{
    private IEnumerable<WondrousTailsTask> wondrousTailsStatus;

    private static WondrousTailsSettings DutyRouletteSettings => Service.ConfigurationManager.CharacterConfiguration.WondrousTails;

    private static bool Enabled => DutyRouletteSettings is {Enabled.Value: true, OverlayEnabled.Value: true};

    private const uint GoldenCloverNodeId = 29;
    private const uint EmptyCloverNodeId = 30;
    
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
        AddonContentsFinder.Instance.Refresh -= OnRefresh;
        AddonContentsFinder.Instance.Update -= OnUpdate;
        AddonContentsFinder.Instance.Draw -= OnDraw;
        AddonContentsFinder.Instance.Finalize -= OnFinalize;
    }

    private void OnRefresh(object? sender, nint addonBase)
    {
        if (!Enabled) return;

        wondrousTailsStatus = WondrousTailsBook.Instance.GetAllTaskData();
    }
    
    private void OnUpdate(object? sender, nint addonBase)
    {
        foreach (var listItem in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            var taskState = IsWondrousTailsDuty(listItem);
    
            if (taskState == null || !WondrousTailsBook.PlayerHasBook || !Enabled)
            {
                SetCloverNodesVisibility(listItem, CloverState.Hidden);
            }
            else if (taskState == ButtonState.Unavailable)
            {
                SetCloverNodesVisibility(listItem, CloverState.Dark);
            }
            else if (taskState is ButtonState.AvailableNow or ButtonState.Completable)
            {
                SetCloverNodesVisibility(listItem, CloverState.Golden);
            }
        }
    }

    private void OnDraw(object? sender, nint addonBase)
    {
        if (!Enabled) return;

        foreach (var listItem in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            var goldenNode = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, GoldenCloverNodeId);
            if (goldenNode is null)
            {
                MakeCloverNode(listItem, GoldenCloverNodeId);
            }

            var emptyNode = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, EmptyCloverNodeId);
            if (emptyNode is null)
            {
                MakeCloverNode(listItem, EmptyCloverNodeId);
            }

            var moogleNode = AddonContentsFinder.GetListItemNode<AtkResNode>(listItem, 6);
            if (moogleNode is not null && moogleNode->X is not 285)
            {
                moogleNode->X = 285;
            }

            var levelSyncNode = AddonContentsFinder.GetListItemNode<AtkResNode>(listItem, 10);
            if (levelSyncNode is not null && levelSyncNode->X is not 305)
            {
                levelSyncNode->X = 305;
            }
        }
    }

    private void OnFinalize(object? sender, nint addonBase)
    {
        foreach (var listItem in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            var goldenNode = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, GoldenCloverNodeId);
            if (goldenNode is not null)
            {
                ImageNode.FreeImageNode(goldenNode);
            }

            var emptyNode = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, EmptyCloverNodeId);
            if (emptyNode is not null)
            {
                ImageNode.FreeImageNode(emptyNode);
            }
        }
    }

    private ButtonState? IsWondrousTailsDuty(nint item)
    {
        var nodeString = AddonContentsFinder.GetListItemString(item);
        var nodeRegexString = AddonContentsFinder.GetListItemFilteredString(item);
        
        var containsEllipsis = nodeString.Contains("...");
    
        foreach (var result in AddonContentsFinder.Instance.Duties)
        {
            if (containsEllipsis)
            {
                var nodeStringLength = nodeRegexString.Length;
    
                if (result.SearchKey.Length <= nodeStringLength) continue;
    
                if (result.SearchKey[..nodeStringLength] == nodeRegexString)
                {
                    return GetWondrousTailsTaskState(result.TerritoryType);
                }
            }
            else if (result.SearchKey == nodeRegexString)
            {
                return GetWondrousTailsTaskState(result.TerritoryType);
            }
        }
    
        return null;
    }

    private ButtonState? GetWondrousTailsTaskState(uint duty) => wondrousTailsStatus.FirstOrDefault(task => task.DutyList.Contains(duty))?.TaskState;
    
    private void SetCloverNodesVisibility(nint listItem, CloverState state)
    {
        var goldenClover = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, GoldenCloverNodeId);
        var emptyClover = AddonContentsFinder.GetListItemNode<AtkImageNode>(listItem, EmptyCloverNodeId);
        
        switch (state)
        {
            case CloverState.Hidden:
                goldenClover->AtkResNode.ToggleVisibility(false);
                emptyClover->AtkResNode.ToggleVisibility(false);
                break;

            case CloverState.Golden:
                goldenClover->AtkResNode.ToggleVisibility(true);
                emptyClover->AtkResNode.ToggleVisibility(false);
                break;

            case CloverState.Dark:
                goldenClover->AtkResNode.ToggleVisibility(false);
                emptyClover->AtkResNode.ToggleVisibility(true);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void MakeCloverNode(nint listItem, uint id)
    {
        if (listItem == nint.Zero) return;
        var listItemNode = (AtkComponentNode*) listItem;

        var textNode = (AtkResNode*) AddonContentsFinder.GetListItemTextNode(listItem);
        if (textNode is null) return;
        
        var textureCoordinates = id == GoldenCloverNodeId ? new Vector2(97, 65) : new Vector2(75, 63);

        var imageNode = ImageNode.MakeNode(id, textureCoordinates, new Vector2(20.0f, 20.0f));
        
        imageNode->LoadTexture("ui/uld/WeeklyBingo.tex");

        imageNode->AtkResNode.ToggleVisibility(true);

        imageNode->AtkResNode.SetWidth(20);
        imageNode->AtkResNode.SetHeight(20);

        var positionOffset = Vector2.Zero;
        
        var xPosition = (short)(325 + positionOffset.X);
        var yPosition = (short)(2 + positionOffset.Y);

        imageNode->AtkResNode.SetPositionShort(xPosition, yPosition);
        
        ImageNode.LinkNode(listItemNode, textNode, imageNode);
    }
}