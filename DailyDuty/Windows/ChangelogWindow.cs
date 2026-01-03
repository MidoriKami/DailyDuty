using System.Linq;
using DailyDuty.Classes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Windows;

public class ChangelogWindow : NativeAddon {
    private ScrollingTreeNode? scrollingTreeNode;
    
    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        scrollingTreeNode = new ScrollingTreeNode {
            Size = ContentSize,
            Position = ContentStartPosition,
            ScrollSpeed = 100,
            AutoHideScrollBar = true,
        };
        scrollingTreeNode.AttachNode(this);

        if (Module is not null) {
            foreach (var changelog in Module.ModuleInfo.ChangeLog.OrderByDescending(log => log.Version)) {
                var categoryNode = new TreeListCategoryNode {
                    String = $"Version {changelog.Version}",
                    OnToggle = _ => scrollingTreeNode.RecalculateLayout(),
                };

                var newTextNode = new TextNode {
                    Width = scrollingTreeNode.TreeListNode.Width,
                    TextFlags = TextFlags.MultiLine | TextFlags.WordWrap,
                    FontSize = 14,
                    LineSpacing = 22,
                    TextColor = ColorHelper.GetColor(1),
                };

                newTextNode.String = changelog.Description;
                newTextNode.Height = newTextNode.GetTextDrawSize(newTextNode.String).Y;
                
                categoryNode.RecalculateLayout();
                
                categoryNode.AddNode(newTextNode);
                scrollingTreeNode.AddCategoryNode(categoryNode);
            }

            scrollingTreeNode.RecalculateLayout();
        }
    }

    public ModuleBase? Module { get; set; }
}
