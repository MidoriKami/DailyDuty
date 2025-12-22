using System;
using System.Numerics;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.Nodes;

public abstract class BasicStatusNode : SimpleComponentNode {
    private readonly TabBarNode tabBarNode;

    private readonly SimpleComponentNode dataNode;
    private readonly SimpleComponentNode statusNode;
    private readonly TextButtonNode changeLogButtonNode;
    private readonly TextButtonNode snoozeButtonNode;
    private readonly TextNode versionNode;

    protected BasicStatusNode() {
        tabBarNode = new TabBarNode();
        tabBarNode.AddTab("Status", OnStatusSelected);
        tabBarNode.AddTab("Data", OnDataSelected);
        tabBarNode.SelectTab("Status");
        tabBarNode.AttachNode(this);

        statusNode = new SimpleComponentNode();
        statusNode.AttachNode(this);
        
        dataNode = new SimpleComponentNode();
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

        versionNode = new TextNode();
        versionNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        tabBarNode.Size = new Vector2(Width - 8.0f, 28.0f);
        tabBarNode.Position = new Vector2(4.0f, 0.0f);
        
        statusNode.Size = new Vector2(Width - 8.0f, Height - 28.0f - 24.0f - 4.0f - 4.0f);
        statusNode.Position = new Vector2(4.0f, tabBarNode.Bounds.Bottom + 4.0f);
        
        dataNode.Size = new Vector2(Width - 8.0f, Height - 28.0f - 24.0f - 4.0f - 4.0f);
        dataNode.Position = new Vector2(4.0f, tabBarNode.Bounds.Bottom + 4.0f);

        changeLogButtonNode.Size = new Vector2(100.0f, 24.0f);
        changeLogButtonNode.Position = new Vector2(4.0f, Height - 24.0f - 4.0f);

        snoozeButtonNode.Size = new Vector2(100.0f, 24.0f);
        snoozeButtonNode.Position = new Vector2(Width / 2.0f - snoozeButtonNode.Width / 2.0f, Height - 24.0f - 4.0f);

        versionNode.Size = new Vector2(100.0f, 24.0f);
        versionNode.Position = new Vector2(Width - versionNode.Width - 4.0f, Height - 24.0f - 4.0f - 4.0f);
    }

    private void OnDataSelected() {
        dataNode.IsVisible = true;
        statusNode.IsVisible = false;
    }

    private void OnStatusSelected() {
        dataNode.IsVisible = false;
        statusNode.IsVisible = true;
    }
    
    private void OpenChangeLogClicked()
        => OpenChangelog?.Invoke();

    private void SnoozeClicked()
        => Snooze?.Invoke();

    public Action? OpenChangelog { get; set; }
    public Action? Snooze { get; set; }

    protected abstract void AttachToDataNode(SimpleComponentNode dataNode);
}
