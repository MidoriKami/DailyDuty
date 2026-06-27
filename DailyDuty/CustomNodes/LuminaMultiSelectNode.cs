using System;
using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Excel;

namespace DailyDuty.CustomNodes;

public class LuminaMultiSelectNode<T> : SimpleComponentNode where T : struct, IExcelRow<T> {
    private readonly ScrollingNode<VerticalListNode> scrollingListNode;

    public LuminaMultiSelectNode() {
        scrollingListNode = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                FitWidth = true,
                FitContents = true,
            },
            AutoHideScrollBar = true,
        };

        scrollingListNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        scrollingListNode.Size = Size;
        scrollingListNode.RecalculateSizes();
    }

    private void OnOptionEdited(T option, bool newValue) {
        if (newValue) {
            if (!Options.Contains(option.RowId)) {
                Options.Add(option.RowId);
            }
        }
        else {
            Options.Remove(option.RowId);
        }

        OnEdited.Invoke();
    }

    public required ICollection<uint> Options {
        get;
        set {
            field = value;

            foreach (var option in Services.DataManager.GetExcelSheet<T>().Where(option => FilterFunc?.Invoke(option) ?? true)) {
                if (GetLabelFunc?.Invoke(option) is not { Length: > 0 } name) continue;

                scrollingListNode.ContentNode.AddNode(new CheckboxNode {
                    Height = 28.0f,
                    String = name,
                    IsChecked = Options.Contains(option.RowId),
                    OnClick = newValue => OnOptionEdited(option, newValue),
                });
            }

            scrollingListNode.RecalculateSizes();
        }
    }

    public required Func<T, string?>? GetLabelFunc { get; set; }
    public Func<T, bool>? FilterFunc { get; set; }
    public required Action OnEdited { get; set; }
}
