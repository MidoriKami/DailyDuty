using System.Numerics;
using DailyDuty.Classes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public abstract class ConfigNodeBase : ResNode;

public abstract class ConfigNodeBase<T> : ConfigNodeBase where T : ModuleBase {

    protected abstract NodeBase? BuildNode();

    protected ConfigNodeBase(T module) {
        notificationSettings = new NotificationSettingsNode<T>(module);
        notificationSettings.AttachNode(this);

        verticalLineNode = new VerticalLineNode {
            Alpha = 0.5f,
        };
        verticalLineNode.AttachNode(this);

        headerNode = new CategoryHeaderNode {
            String = Strings.ConfigNodeBase_ModuleSettings,
        };
        headerNode.AttachNode(this);

        configContentNode = new ResNode {
            IsVisible = false,
        };
        configContentNode.AttachNode(this);

        noOptionsTextNode = new TextNode {
            String = Strings.ConfigNodeBase_NoOptions,
            AlignmentType = AlignmentType.Top,
            Height = 32.0f,
        };
        noOptionsTextNode.AttachNode(this);

        AttachDataNode();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var regionSize = Width / 2.0f - 6.0f;

        verticalLineNode.Size = new Vector2(Height - 28.0f, 4.0f);
        verticalLineNode.Position = new Vector2(regionSize + 4.0f, 28.0f);

        notificationSettings.Size = new Vector2(regionSize, Height);
        notificationSettings.Position = new Vector2(0.0f, 0.0f);

        headerNode.Size = new Vector2(regionSize, 40.0f);
        headerNode.Position = new Vector2(regionSize + 6.0f, 0.0f);

        configContentNode.Size = new Vector2(regionSize, Height - headerNode.Height);
        configContentNode.Position = new Vector2(regionSize + 6.0f, headerNode.Bounds.Bottom);

        noOptionsTextNode.Size = configContentNode.Size;
        noOptionsTextNode.Position = configContentNode.Position;

        builtNode?.Size = configContentNode.Size;
        builtNode?.Position = new Vector2(0.0f, 0.0f);

        if (builtNode is ILayoutListNode layoutNode) {
            layoutNode.RecalculateLayout();
        }
    }

    private void AttachDataNode() {
        builtNode = BuildNode();

        builtNode?.Size = configContentNode.Size;
        builtNode?.AttachNode(configContentNode);

        if (builtNode is ILayoutListNode layoutNode) {
            layoutNode.RecalculateLayout();
        }

        noOptionsTextNode.IsVisible = builtNode is null;
        configContentNode.IsVisible = builtNode is not null;
    }

    private readonly NotificationSettingsNode<T> notificationSettings;
    private readonly ResNode configContentNode;
    private readonly VerticalLineNode verticalLineNode;
    private readonly CategoryHeaderNode headerNode;
    private readonly TextNode noOptionsTextNode;
    private NodeBase? builtNode;
}
