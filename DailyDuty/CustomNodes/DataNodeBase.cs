using System;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public abstract class DataNodeBase : UpdatableNode {
    public abstract Action<DataNodeTab>? TabSelected { get; set; }
    public abstract void SelectTab(DataNodeTab tab);
}

public abstract class DataNodeBase<T> : DataNodeBase where T : ModuleBase {
    private readonly T module;
    private readonly TabBarNode tabBarNode;
    private readonly CategoryHeaderNode categoryHeaderNode;

    private readonly SimpleComponentNode footerNode;
    private readonly NodeBase dataNode;
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
            String = "Module Data",
            Alignment = AlignmentType.Bottom,
        };
        categoryHeaderNode.AttachNode(dataContentSection);

        dataNode = GetDataNode();
        dataNode.AttachNode(dataContentSection);

        footerNode = new SimpleComponentNode();
        footerNode.AttachNode(this);
        
        changeLogButtonNode = new TextButtonNode {
            String = "Changelog",
            OnClick = OpenChangeLogClicked,
        };
        changeLogButtonNode.AttachNode(footerNode);

        snoozeButtonNode = new TextButtonNode {
            String = module.ConfigBase.Suppressed ? "Unsnooze" : "Snooze",
            IsEnabled = module.DataBase.NextReset != DateTime.MaxValue,
            OnClick = SnoozeClicked,
            TextTooltip = module.ConfigBase.Suppressed ? string.Empty : "Suppresses notification until the next reset",
        };
        snoozeButtonNode.AttachNode(footerNode);

        versionNode = new TextNode {
            AlignmentType = AlignmentType.BottomRight,
            String = $"Version {module.ModuleInfo.Version}",
        };
        versionNode.AttachNode(footerNode);
        
        OnStatusSelected();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        var buttonSize = new Vector2(130.0f, 28.0f);
        const float padding = 4.0f;
        var widthPadded = Width - padding * 2.0f;

        tabBarNode.Size = new Vector2(widthPadded, 20.0f);
        tabBarNode.Position = new Vector2(padding, 0.0f);
        
        footerNode.Size = new Vector2(widthPadded, 24.0f);
        footerNode.Position = new Vector2(padding, Height - footerNode.Height - padding);

        statusDisplayNode.Size = new Vector2(widthPadded, Height - tabBarNode.Bounds.Bottom - footerNode.Height - padding * 3.0f);
        statusDisplayNode.Position = new Vector2(padding, tabBarNode.Bounds.Bottom + padding);

        dataContentSection.Size = statusDisplayNode.Size;
        dataContentSection.Position = statusDisplayNode.Position;

        categoryHeaderNode.Size = new Vector2(widthPadded, 40.0f);
        categoryHeaderNode.Position = Vector2.Zero;
        
        dataNode.Size = new Vector2(dataContentSection.Width, dataContentSection.Height - categoryHeaderNode.Height - padding);
        dataNode.Position = new Vector2(0.0f, categoryHeaderNode.Bounds.Bottom + padding);

        if (dataNode is LayoutListNode layoutNode) {
            layoutNode.RecalculateLayout();
        }
        
        changeLogButtonNode.Size = buttonSize;
        changeLogButtonNode.Position = Vector2.Zero;
        
        snoozeButtonNode.Size = buttonSize;
        snoozeButtonNode.Position = new Vector2(footerNode.Width / 2.0f - buttonSize.X / 2.0f, 0.0f);

        versionNode.Size = buttonSize;
        versionNode.Position = new Vector2(footerNode.Width - buttonSize.X, 0.0f);
    }

    public override void Update() {
        statusDisplayNode.Update(module);

        snoozeButtonNode.IsEnabled = module.ModuleStatus is not (CompletionStatus.Complete or CompletionStatus.Disabled);
    }
    
    private void OnDataSelected() {
        dataContentSection.IsVisible = true;
        statusDisplayNode.IsVisible = false;
        
        TabSelected?.Invoke(DataNodeTab.Data);
    }

    private void OnStatusSelected() {
        dataContentSection.IsVisible = false;
        statusDisplayNode.IsVisible = true;
        
        TabSelected?.Invoke(DataNodeTab.Status);
    }
    
    private void OpenChangeLogClicked() {
        module.ChangelogWindow ??= new ChangelogWindow {
            InternalName = "DailyDutyChangelog",
            Title = "Changelog",
            Size = new Vector2(450.0f, 400.0f),
            Module = module,
        };
        
        module.ChangelogWindow.Toggle();
    }

    private void SnoozeClicked() {
        module.ConfigBase.Suppressed = !module.ConfigBase.Suppressed;
        
        snoozeButtonNode.String = module.ConfigBase.Suppressed ? "Unsnooze" : "Snooze";
        if (!module.ConfigBase.Suppressed) {
            snoozeButtonNode.TextTooltip = "Suppresses notification until the next reset";
        }
        else {
            snoozeButtonNode.HideTooltip();
            snoozeButtonNode.TextTooltip = string.Empty;
        }
        
        module.ConfigBase.MarkDirty();
    }

    protected abstract NodeBase BuildDataNode();

    // Workaround for complaining about virtual call in ctor,
    // When BuildDataNode does not require child class to be constructed first.
    private NodeBase GetDataNode() => BuildDataNode();

    public sealed override Action<DataNodeTab>? TabSelected { get; set; }

    public sealed override void SelectTab(DataNodeTab tab) {
        switch (tab) {
            case DataNodeTab.Status:
                tabBarNode.SelectTab("Status");
                OnStatusSelected();
                break;
            
            case DataNodeTab.Data:
                tabBarNode.SelectTab("Data");
                OnDataSelected();
                break;
        }
    }
}
