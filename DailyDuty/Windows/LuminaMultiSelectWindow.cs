using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Excel;

namespace DailyDuty.Windows;

public class LuminaMultiSelectWindow<T> : NativeAddon where T : struct, IExcelRow<T> {

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

        foreach (var option in Services.DataManager.GetExcelSheet<T>()) {
            if (GetLabelFunc?.Invoke(option) is not { Length: > 0 } name) continue;
            
            scrollable.ContentNode.AddNode(new CheckboxNode {
                Height = 24.0f,
                String = name,
                IsChecked = Options.Contains(option.RowId),
                OnClick = newValue => {
                    if (newValue) {
                        if (!Options.Contains(option.RowId)) {
                            Options.Add(option.RowId);
                        }
                    }
                    else {
                        Options.Remove(option.RowId);
                    }
                },
            });
        }

        scrollable.ContentHeight = scrollable.ContentNode.Height;
        
        scrollable.AttachNode(this);
    }
    
    public required ICollection<uint> Options { get; set; }
    public required Func<T, string?>? GetLabelFunc { get; set; }
}
