using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.AddonOverlays;

public unsafe class DutyRouletteOverlay : IDisposable
{
    private static DutyRouletteSettings RouletteSettings => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette;
    private static IEnumerable<TrackedRoulette> DutyRoulettes => RouletteSettings.TrackedRoulettes;

    private static bool Enabled => RouletteSettings is {Enabled.Value: true, OverlayEnabled.Value: true};
    
    private bool defaultColorSaved;
    private ByteColor userDefaultTextColor;
    
    public DutyRouletteOverlay()
    {
        AddonContentsFinder.Instance.Draw += OnDraw;
        AddonContentsFinder.Instance.Finalize += OnFinalize;
    }

    public void Dispose()
    {
        AddonContentsFinder.Instance.Draw -= OnDraw;
        AddonContentsFinder.Instance.Finalize -= OnFinalize;
    }
 
    private void OnDraw(object? sender, nint addonBase)
    {
        SaveUserDefaultColor(addonBase);

        if (Enabled && defaultColorSaved && AddonContentsFinder.GetSelectedTab(addonBase) is 0)
        {
            SetRouletteColors(addonBase);
        }
        else if (defaultColorSaved)
        {
            ResetDefaultTextColor(addonBase);
        }
    }

    private void OnFinalize(object? sender, nint addonBase)
    {
        if (defaultColorSaved)
        {
            ResetDefaultTextColor(addonBase);
        }
    }

    private void SaveUserDefaultColor(nint addonBase)
    {
        if (defaultColorSaved) return;
        
        foreach (var item in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            if (item == nint.Zero) continue;

            var textNode = AddonContentsFinder.GetListItemTextNode(item);
            if (textNode is not null)
            {
                var nodeColor = GetVectorColor(textNode->TextColor);
                        
                // If the color is one of our presets, skip.
                if (nodeColor == RouletteSettings.CompleteColor.Value) continue;
                if (nodeColor == RouletteSettings.IncompleteColor.Value) continue;
                if (nodeColor == RouletteSettings.OverrideColor.Value) continue;
                if (nodeColor == Vector4.Zero) continue; // Blank
                        
                userDefaultTextColor = textNode->TextColor;
                defaultColorSaved = true;
                return;
            }
        }
    }

    private void ResetDefaultTextColor(nint addonBase)
    {
        foreach (var listItemNode in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            var textNode = AddonContentsFinder.GetListItemTextNode(listItemNode);
            if (textNode is not null)
            {
                SetTextNodeColor(textNode, userDefaultTextColor);
            }
        }
    }

    private void SetRouletteColors(nint addonBase)
    {
        foreach (var item in AddonContentsFinder.GetDutyListItems(addonBase))
        {
            var textNode = AddonContentsFinder.GetListItemTextNode(item);
            var listItemText = AddonContentsFinder.FilterString(textNode);
            
            if (IsRouletteDuty(listItemText) is { } trackedRoulette)
            {
                switch (trackedRoulette)
                {
                    case { Tracked.Value: true, State: RouletteState.Complete }:
                        SetTextNodeColor(textNode, RouletteSettings.CompleteColor.Value);
                        break;
        
                    case { Tracked.Value: true, State: RouletteState.Incomplete }:
                        SetTextNodeColor(textNode, RouletteSettings.IncompleteColor.Value);
                        break;
        
                    case { Tracked.Value: true, State: RouletteState.Overriden }:
                        SetTextNodeColor(textNode, RouletteSettings.OverrideColor.Value);
                        break;
        
                    default:
                        SetTextNodeColor(textNode, userDefaultTextColor);
                        break;
                }
            }
        }
    }

    private static TrackedRoulette? IsRouletteDuty(string filteredString)
    {
        var dutyFinderResult = AddonContentsFinder.Instance.Roulettes.FirstOrDefault(duty => duty.SearchKey == filteredString);
        if (dutyFinderResult == null) return null;
    
        return DutyRoulettes.FirstOrDefault(duty => (uint) duty.Roulette == dutyFinderResult.TerritoryType);
    }

    private void SetTextNodeColor(AtkTextNode* textNode, Vector4 color) => SetTextNodeColor(textNode, GetByteColor(color));

    private void SetTextNodeColor(AtkTextNode* textNode, ByteColor color)
    {
        textNode->TextColor.R = color.R;
        textNode->TextColor.G = color.G;
        textNode->TextColor.B = color.B;
        textNode->TextColor.A = color.A;
    }

    private static ByteColor GetByteColor(Vector4 vectorColor)
    {
        return new ByteColor
        {
            A = (byte) (vectorColor.W * 255),
            R = (byte) (vectorColor.X * 255),
            G = (byte) (vectorColor.Y * 255),
            B = (byte) (vectorColor.Z * 255),
        };
    }

    private static Vector4 GetVectorColor(ByteColor byteColor)
    {
        return new Vector4
        {
            W = byteColor.A / 255.0f,
            X = byteColor.R / 255.0f,
            Y = byteColor.G / 255.0f,
            Z = byteColor.B / 255.0f,
        };
    }
}