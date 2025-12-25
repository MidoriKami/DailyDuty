using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Interfaces;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig;

public unsafe class ConfigAddon : NativeAddon {
    private ScrollingAreaNode<VerticalListNode>? configurationListNode;

    private readonly List<ConfigCategory> configCategories = [];
    
    public required ISavable Config { get; init; }

    private const float MaximumHeight = 400.0f;
    private const float Width = 400.0f;

    protected override void OnSetup(AtkUnitBase* addon) {
        configurationListNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = ContentSize.Y,
            AutoHideScrollBar = true,
        };
        configurationListNode.AttachNode(this);

        foreach (var category in configCategories) {
            configurationListNode.ContentNode.AddNode(category.BuildNode());
        }
        RecalculateWindowSize();
    }

    private void RecalculateWindowSize() {
        if (configurationListNode is null) return;

        configurationListNode.ContentHeight = configurationListNode.ContentNode.Nodes.Sum(node => node.Height) + 10.0f;

        if (configurationListNode.ContentHeight < MaximumHeight) {
            Size = new Vector2(Width, configurationListNode.ContentHeight + ContentStartPosition.Y + 24.0f);
        }
        else {
            Size = new Vector2(Width, MaximumHeight + ContentStartPosition.Y + 24.0f);
        }
        
        SetWindowSize(Size);
        
        configurationListNode.Size = ContentSize + new Vector2(0.0f, ContentPadding.Y);
        configurationListNode.Position = ContentStartPosition - new Vector2(0.0f, ContentPadding.Y);
        configurationListNode.ContentNode.RecalculateLayout();

        foreach (var node in configurationListNode.ContentNode.GetNodes<TabbedVerticalListNode>()) {
            node.Width = configurationListNode.ContentNode.Width;
            node.RecalculateLayout();
        }
    }

    public ConfigCategory AddCategory(string label) {
        var newCategory = new ConfigCategory {
            CategoryLabel = label,
            ConfigObject = Config,
        };
        
        configCategories.Add(newCategory);
        return newCategory;
    }

    public override void Dispose() {
        base.Dispose();

        foreach (var category in configCategories) {
            category.Dispose();
        }
    }
}
