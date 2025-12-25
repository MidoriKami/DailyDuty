using System;
using System.Numerics;
using DailyDuty.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.Nodes;

public abstract class StatusNode<T> : UpdatableNode where T : ModuleBase {
    private readonly ChangelogWindow? changelogWindow = new() {
        InternalName = "DailyDutyChangelog",
        Title = "Changelog",
        Size = new Vector2(450.0f, 400.0f),
    };
    
    private readonly T module;
    private readonly TabBarNode tabBarNode;

    private readonly ScrollingAreaNode<VerticalListNode> dataNode;
    private readonly TextButtonNode changeLogButtonNode;
    private readonly TextButtonNode snoozeButtonNode;
    private readonly TextNode versionNode;

    private readonly GenericDataNode statusDisplayNode;

    protected StatusNode(T module) {
        this.module = module;

        tabBarNode = new TabBarNode();
        tabBarNode.AddTab("Status", OnStatusSelected);
        tabBarNode.AddTab("Data", OnDataSelected);
        tabBarNode.SelectTab("Status");
        tabBarNode.AttachNode(this);

        statusDisplayNode = new GenericDataNode();
        statusDisplayNode.AttachNode(this);

        dataNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 1000.0f,
            AutoHideScrollBar = true,
        };
        dataNode.ContentNode.FitContents = true;
        dataNode.ContentNode.FitWidth = true;
        AttachDataNode(dataNode.ContentNode);
        dataNode.AttachNode(this);

        changeLogButtonNode = new TextButtonNode {
            String = "Changelog",
            OnClick = OpenChangeLogClicked,
        };
        changeLogButtonNode.AttachNode(this);

        snoozeButtonNode = new TextButtonNode {
            String = "Snooze",
            OnClick = SnoozeClicked,
        };
        snoozeButtonNode.AttachNode(this);

        versionNode = new TextNode {
            String = "Version -69.420",
        };
        versionNode.AttachNode(this);
        
        OnStatusSelected();
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        base.Dispose(disposing, isNativeDestructor);
        
        changelogWindow?.Dispose();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var buttonSize = new Vector2(130.0f, 24.0f);
        const float padding = 4.0f;
        
        tabBarNode.Size = new Vector2(Width - padding * 2.0f, 20.0f);
        tabBarNode.Position = new Vector2(padding, 0.0f);

        changeLogButtonNode.Size = buttonSize;
        changeLogButtonNode.Position = new Vector2(padding, Height - buttonSize.Y - padding);
        
        snoozeButtonNode.Size = buttonSize;
        snoozeButtonNode.Position = new Vector2(Width / 2.0f - snoozeButtonNode.Width / 2.0f, Height - buttonSize.Y - padding);
        
        versionNode.Size = new Vector2(100.0f, 24.0f);
        versionNode.Position = new Vector2(Width - versionNode.Width - 4.0f, Height - 16.0f - 4.0f - 4.0f);

        var contentsSize = new Vector2(Width - padding * 2.0f, Height - tabBarNode.Height - buttonSize.Y - padding * 5.0f);
        var contentPosition = new Vector2(padding, tabBarNode.Bounds.Bottom + padding * 2.0f);

        statusDisplayNode.Size = contentsSize;
        statusDisplayNode.Position = contentPosition;

        dataNode.Size = contentsSize;
        dataNode.Position = contentPosition;
        dataNode.ContentNode.Width = Width;
        dataNode.ContentNode.RecalculateLayout();
    }

    public override void Update() {
        statusDisplayNode.Update(module);
    }

    private void OnDataSelected() {
        dataNode.IsVisible = true;
        statusDisplayNode.IsVisible = false;
    }

    private void OnStatusSelected() {
        dataNode.IsVisible = false;
        statusDisplayNode.IsVisible = true;
    }
    
    private void OpenChangeLogClicked() {
        changelogWindow?.Module = module;
        changelogWindow?.Toggle();
    }

    private void SnoozeClicked()
        => Snooze?.Invoke();

    public Action? Snooze { get; set; }

    protected abstract void BuildNode(VerticalListNode container);
    
    private void AttachDataNode(VerticalListNode container) {
        BuildNode(container);
        container.RecalculateLayout();
    }
}
