using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;

namespace DailyDuty.Windows;

public class MultiSelectWindow : NativeAddon {
    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> _) {
        var scrollable = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                FitWidth = true,
                FitContents = true,
            },
            AutoHideScrollBar = true,
            Size = ContentSize,
            Position = ContentStartPosition,
        };

        foreach (var option in Options) {
            scrollable.ContentNode.AddNode(new CheckboxNode {
                Height = 28.0f,
                String = option,
                IsChecked = SelectedOptions.Contains(option),
                OnClick = newValue => OnOptionEdited(option, newValue),
            });
        }

        scrollable.RecalculateSizes();
        scrollable.AttachNode(this);
    }

    private void OnOptionEdited(string option, bool newValue) {
        if (newValue) {
            if (!SelectedOptions.Contains(option)) {
                SelectedOptions.Add(option);
            }
        }
        else {
            SelectedOptions.Remove(option);
        }

        OnEdited.Invoke();
    }

    public required ICollection<string> Options { get; set; }
    public required ICollection<string> SelectedOptions { get; set; }
    public required Action OnEdited { get; set; }
}
