using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.Windows;

public class MultiSelectWindow : NativeAddon {
    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        var scrollable = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = ContentSize.Y,
            AutoHideScrollBar = true,
            Size = ContentSize,
            Position = ContentStartPosition,
        };

        scrollable.ContentNode.FitWidth = true;
        scrollable.ContentNode.FitContents = true;
        scrollable.ContentNode.RecalculateLayout();

        foreach (var option in Options) {
            scrollable.ContentNode.AddNode(new CheckboxNode {
                Height = 24.0f,
                String = option,
                IsChecked = SelectedOptions.Contains(option),
                OnClick = newValue => OnOptionEdited(option, newValue),
            });
        }

        scrollable.ContentHeight = scrollable.ContentNode.Height;
        
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
