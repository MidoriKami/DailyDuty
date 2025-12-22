using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow;

public class ChangelogWindow : NativeAddon {
    private ScrollingAreaNode<TreeListNode>? scrollingAreaNode;
    
    private readonly List<TreeListCategoryNode> categoryNodes = [];

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        scrollingAreaNode = new ScrollingAreaNode<TreeListNode> {
            Size = ContentSize,
            Position = ContentStartPosition,
            ContentHeight = 1000.0f,
            ScrollSpeed = 100,
            AutoHideScrollBar = true,
        };
        scrollingAreaNode.AttachNode(this);

        if (Modification is not null) {
            categoryNodes.Clear();

            foreach (var changelog in Modification.ModuleInfo.ChangeLog.OrderByDescending(log => log.Version)) {
                var categoryNode = new TreeListCategoryNode {
                    SeString = $"Version {changelog.Version}",
                    Width = ContentSize.X,
                    OnToggle = _ => scrollingAreaNode.ContentHeight = categoryNodes.Sum(node => node.Height),
                };

                var newTextNode = new TextNode {
                    Height = 32.0f,
                    AlignmentType = AlignmentType.TopLeft,
                    TextFlags = TextFlags.MultiLine | TextFlags.WordWrap | TextFlags.AutoAdjustNodeSize,
                    Position = new Vector2(6.0f, 2.0f),
                    Width = ContentSize.X - 32.0f,
                    FontSize = 14,
                    LineSpacing = 22,
                    String = changelog.Description,
                    TextColor = ColorHelper.GetColor(1),
                };

                newTextNode.Height = newTextNode.GetTextDrawSize(newTextNode.SeString).Y;
                
                categoryNode.AddNode(newTextNode);

                scrollingAreaNode.ContentNode.AddCategoryNode(categoryNode);
                categoryNodes.Add(categoryNode);
            }

            scrollingAreaNode.ContentHeight = categoryNodes.Sum(node => node.Height);
        }
    }

    public ModuleBase? Modification { get; set; }
}
