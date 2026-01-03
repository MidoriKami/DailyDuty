using System.Numerics;
using DailyDuty.Classes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public abstract class ConfigNodeBase : SimpleComponentNode;

public abstract class ConfigNodeBase<T> : ConfigNodeBase where T : ModuleBase {
    
    private readonly TabBarNode tabBar;
    private readonly NotificationSettingsNode<T> notificationSettings;
    private readonly ScrollingListNode configNode;

    protected ConfigNodeBase(T module) {
        tabBar = new TabBarNode();
        tabBar.AddTab("Notification", OnNotificationTabSelected);
        tabBar.AddTab("Settings", OnSettingsTabSelected);
        tabBar.AttachNode(this);

        notificationSettings = new NotificationSettingsNode<T>(module);
        notificationSettings.AttachNode(this);
    
        configNode = new ScrollingListNode {
            AutoHideScrollBar = true,
            IsVisible = false,
        };
        configNode.FitContents = true;
        configNode.FitWidth = true;
        configNode.ItemSpacing = 4.0f;

        configNode.AddNode(new CategoryHeaderNode {
            Label = "Module Settings",
        });
        
        AttachDataNode(configNode);
        configNode.AttachNode(this);
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        tabBar.Size = new Vector2(Width, 24.0f);
        tabBar.Position = new Vector2(0.0f, 0.0f);
        
        notificationSettings.Size = Size - new Vector2(0.0f, 28.0f);
        notificationSettings.Position = new Vector2(0.0f, 28.0f);
        
        configNode.Size = Size - new Vector2(0.0f, 28.0f);
        configNode.Position = new Vector2(0.0f, 28.0f);
        configNode.RecalculateLayout();
    }
    
    private void OnNotificationTabSelected() {
        notificationSettings.IsVisible = true;
        configNode.IsVisible = false;
    }
    
    private void OnSettingsTabSelected() {
        notificationSettings.IsVisible = false;
        configNode.IsVisible = true;
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
    }
}
