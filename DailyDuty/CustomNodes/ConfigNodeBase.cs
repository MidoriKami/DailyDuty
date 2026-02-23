using System.Numerics;
using DailyDuty.Classes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public abstract class ConfigNodeBase : SimpleComponentNode;

public abstract class ConfigNodeBase<T> : ConfigNodeBase where T : ModuleBase {
    
    private readonly NotificationSettingsNode<T> notificationSettings;
    private readonly ScrollingListNode configNode;
    private readonly VerticalLineNode verticalLineNode;

    protected ConfigNodeBase(T module) {
        notificationSettings = new NotificationSettingsNode<T>(module);
        notificationSettings.AttachNode(this);

        verticalLineNode = new VerticalLineNode {
            Alpha = 0.5f,
        };
        verticalLineNode.AttachNode(this);
        
        configNode = new ScrollingListNode {
            AutoHideScrollBar = true,
            IsVisible = false,
        };
        configNode.FitContents = true;
        configNode.FitWidth = true;
        configNode.ItemSpacing = 4.0f;

        configNode.AddNode(new CategoryHeaderNode {
            String = "Module Settings",
        });
        
        AttachDataNode(configNode);
        configNode.AttachNode(this);
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var regionSize = Width / 2.0f - 6.0f;
        
        verticalLineNode.Size = new Vector2(Height - 28.0f, 4.0f);
        verticalLineNode.Position = new Vector2(regionSize + 4.0f, 28.0f);

        notificationSettings.Size = new Vector2(regionSize, Height);
        notificationSettings.Position = new Vector2(0.0f, 0.0f);

        configNode.Size = new Vector2(regionSize, Height);
        configNode.Position = new Vector2(regionSize + 6.0f, 0.0f);
        configNode.RecalculateLayout();
    }
    
    protected abstract void BuildNode(ScrollingListNode container);
    
    private void AttachDataNode(ScrollingListNode container) {
        var preCount = container.Nodes.Count;
        
        BuildNode(container);
        container.RecalculateLayout();
        
        var postCount = container.Nodes.Count;

        if (preCount == postCount) {
            container.AddNode(new TextNode {
                String = "No options available for this module",
                AlignmentType = AlignmentType.Bottom,
                Height = 32.0f,
            });
        }

        container.IsVisible = true;
    }
}
