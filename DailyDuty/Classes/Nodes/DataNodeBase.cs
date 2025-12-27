using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public abstract class DataNodeBase : UpdatableNode;

public abstract class DataNodeBase<T> : DataNodeBase where T : ModuleBase {
    private readonly ChangelogWindow? changelogWindow = new() {
        InternalName = "DailyDutyChangelog",
        Title = "Changelog",
        Size = new Vector2(450.0f, 400.0f),
    };
    
    private readonly T module;
    private readonly TabBarNode tabBarNode;
    private readonly CategoryHeaderNode categoryHeaderNode;

    private readonly ScrollingAreaNode<VerticalListNode> dataNode;
    private readonly TextButtonNode changeLogButtonNode;
    private readonly TextButtonNode snoozeButtonNode;
    private readonly TextNode versionNode;

    private readonly SimpleComponentNode dataContentSection;
    private readonly GenericDataNode statusDisplayNode;

    protected DataNodeBase(T module) {
        this.module = module;
        
        tabBarNode = new TabBarNode();
        tabBarNode.AddTab("Status", OnStatusSelected);
        tabBarNode.AddTab("Data", OnDataSelected);
        tabBarNode.SelectTab("Status");
        tabBarNode.AttachNode(this);

        statusDisplayNode = new GenericDataNode();
        statusDisplayNode.AttachNode(this);

        dataContentSection = new SimpleComponentNode();
        dataContentSection.AttachNode(this);

        categoryHeaderNode = new CategoryHeaderNode {
            Label = "Module Data",
            Alignment = AlignmentType.Bottom,
        };
        categoryHeaderNode.AttachNode(dataContentSection);

        dataNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 1000.0f,
            AutoHideScrollBar = true,
        };
        dataNode.ContentNode.FitContents = true;
        dataNode.ContentNode.FitWidth = true;
        
        AttachDataNode(dataNode.ContentNode);
        dataNode.AttachNode(dataContentSection);

        changeLogButtonNode = new TextButtonNode {
            String = "Changelog",
            OnClick = OpenChangeLogClicked,
        };
        changeLogButtonNode.AttachNode(this);

        snoozeButtonNode = new TextButtonNode {
            String = module.ConfigBase.Suppressed ? "Unsnooze" : "Snooze",
            OnClick = SnoozeClicked,
        };
        snoozeButtonNode.AttachNode(this);

        versionNode = new TextNode {
            AlignmentType = AlignmentType.BottomRight,
            String = $"Version {module.ModuleInfo.Version}",
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

        categoryHeaderNode.Size = new Vector2(Width - padding, 40.0f);
        categoryHeaderNode.Position = new Vector2(padding, tabBarNode.Bounds.Bottom + padding * 2.0f);
        
        changeLogButtonNode.Size = buttonSize;
        changeLogButtonNode.Position = new Vector2(padding, Height - buttonSize.Y - padding);
        
        snoozeButtonNode.Size = buttonSize;
        snoozeButtonNode.Position = new Vector2(Width / 2.0f - snoozeButtonNode.Width / 2.0f, Height - buttonSize.Y - padding);
        
        versionNode.Size = buttonSize;
        versionNode.Position = new Vector2(Width - versionNode.Width - 4.0f, Height - buttonSize.Y - padding);

        var contentsSize = new Vector2(Width - padding * 2.0f, Height - tabBarNode.Height - buttonSize.Y - padding * 5.0f - 44.0f);
        var contentPosition = new Vector2(padding, tabBarNode.Bounds.Bottom + padding * 2.0f + 44.0f);

        statusDisplayNode.Size = contentsSize;
        statusDisplayNode.Position = contentPosition;

        dataNode.Size = contentsSize;
        dataNode.Position = contentPosition;
        dataNode.ContentNode.RecalculateLayout();
    }

    public override void Update() {
        statusDisplayNode.Update(module);

        snoozeButtonNode.IsEnabled = module.ModuleStatus is not CompletionStatus.Complete;
    }

    private void OnDataSelected() {
        dataContentSection.IsVisible = true;
        statusDisplayNode.IsVisible = false;
    }

    private void OnStatusSelected() {
        dataContentSection.IsVisible = false;
        statusDisplayNode.IsVisible = true;
    }
    
    private void OpenChangeLogClicked() {
        changelogWindow?.Module = module;
        changelogWindow?.Toggle();
    }

    private void SnoozeClicked() {
        module.ConfigBase.Suppressed = !module.ConfigBase.Suppressed;
        
        snoozeButtonNode.String = module.ConfigBase.Suppressed ? "Unsnooze" : "Snooze";
        
        module.ConfigBase.SavePending = true;
    }

    protected abstract void BuildNode(VerticalListNode container);
    
    private void AttachDataNode(VerticalListNode container) {
        BuildNode(container);
        container.RecalculateLayout();
    }
}
