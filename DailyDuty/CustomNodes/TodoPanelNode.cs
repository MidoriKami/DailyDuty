using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features.TodoOverlay;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Overlay;
using KamiToolKit.Extensions;

namespace DailyDuty.CustomNodes;

public unsafe class TodoPanelNode : OverlayNode {
    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;
    private readonly WindowBackgroundNode frame;
    private readonly ImageNode backgroundImage;
    private readonly WindowBackgroundNode frameFront;
    private readonly TextNode titleText;
    private readonly HorizontalLineNode horizontalLine;
    private readonly VerticalListNode warningList;
    private readonly CircleButtonNode configButton;
    private readonly CircleButtonNode collapseButton;
    
    private TodoOverlayPanelConfigWindow? configWindow;

    public required TodoPanelConfig Config { get; init; }
    public required TodoOverlayConfig ModuleTodoOverlayConfig { get; init; }
    
    public TodoPanelNode() {
        frame = new WindowBackgroundNode(false) {
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Fill,
            PartsRenderType = 19,
        };
        frame.AttachNode(this);
        
        backgroundImage = new SimpleImageNode {
            WrapMode = WrapMode.Stretch,
            TexturePath = "ui/uld/WindowA_Gradation.tex",
            TextureCoordinates = new Vector2(6.0f, 2.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
        };
        backgroundImage.AttachNode(this);
        
        frameFront = new WindowBackgroundNode(true) {
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Fill,
            PartsRenderType = 19,
        };
        frameFront.AttachNode(this);

        titleText = new TextNode {
            FontType = FontType.TrumpGothic,
            FontSize = 23,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(50),
            TextOutlineColor = ColorHelper.GetColor(54),
            TextFlags = TextFlags.Edge,
            String = "Important",
        };
        titleText.AttachNode(this);

        horizontalLine = new HorizontalLineNode();
        horizontalLine.AttachNode(this);

        warningList = new VerticalListNode {
            FitContents = true,
        };
        warningList.AttachNode(this);

        configButton = new CircleButtonNode {
            Icon = ButtonIcon.GearCog,
            OnClick = OpenConfig,
        };
        configButton.AttachNode(this);

        collapseButton = new CircleButtonNode {
            Icon = ButtonIcon.Eye,
            OnClick = () => {
                Config?.IsCollapsed = !Config.IsCollapsed;
                ModuleTodoOverlayConfig?.MarkDirty();
            },
        };
        collapseButton.AttachNode(this);
        
        OnMoveComplete = _ => {
            Config?.Position = Position;
            ModuleTodoOverlayConfig?.MarkDirty();
        };
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        frame.Size = Size;
        frameFront.Size = Size;
        
        backgroundImage.Size = new Vector2(Width - 8.0f, Height - 16.0f);
        backgroundImage.Position = new Vector2(4.0f, 4.0f);

        titleText.Size = new Vector2(100.0f, 31.0f);
        titleText.Position = new Vector2(12.0f, 7.0f);

        configButton.Size = new Vector2(32.0f, 32.0f);
        configButton.Position = new Vector2(Width - 36.0f, 7.0f);

        collapseButton.Size = new Vector2(32.0f, 32.0f);
        collapseButton.Position = new Vector2(Width - 36.0f - configButton.Width, 7.0f);
        
        horizontalLine.Size = new Vector2(Width - 16.0f, 4.0f);
        horizontalLine.Position = new Vector2(8.0f, titleText.Bounds.Bottom + 2.0f);

        warningList.Width = Width - 32.0f;
        warningList.Position = new Vector2(16.0f, horizontalLine.Bounds.Bottom + 12.0f);
        warningList.RecalculateLayout();
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        base.Dispose(disposing, isNativeDestructor);
        
        configWindow?.Dispose();
        configWindow = null;
    }

    protected override void OnUpdate() {
        if (Config.AttachToQuestList) {
            var todoAddon = RaptureAtkUnitManager.Instance()->GetAddonByName("_ToDoList");
            if (todoAddon is not null) {
                Position = todoAddon->Position + todoAddon->RootSize * todoAddon->Scale - new Vector2(Width, 0.0f);
            }
        }
        
        EnableMoving = Config.EnableMoving;

        frameFront.Alpha = Config.Alpha;
        frame.Alpha = Config.Alpha;
        backgroundImage.Alpha = Config.Alpha;

        titleText.String = Config.Label;

        if (Config.Alignment != warningList.Alignment) {
            warningList.Alignment = Config.Alignment;
            warningList.RecalculateLayout();
        }

        frameFront.IsVisible = Config is { ShowFrame: true, IsCollapsed: false };
        frame.IsVisible = Config is { ShowFrame: true, IsCollapsed: false };
        backgroundImage.IsVisible = Config is { ShowFrame: true, IsCollapsed: false };
        
        warningList.IsVisible = !Config.IsCollapsed;
        warningList.ItemSpacing = Config.ItemSpacing;

        var warningModules = Config.Modules.Select(moduleName => System.ModuleManager.GetModule(moduleName))
            .OfType<ModuleBase>()
            .Where(module => module.ModuleStatus is CompletionStatus.Incomplete)
            .OrderBy(module => module.Name)
            .ToList();

        IsVisible = warningModules.Count is not 0 || Config.Modules.Count is 0;

        if (warningList.SyncWithListData(warningModules, node => node.Module, BuildTodoEntry)) {
            warningList.Width = MathF.Max(50.0f, warningList.Nodes.Sum(node => node.IsVisible ? node.Width : 0.0f));
            warningList.RecalculateLayout();

            Height = warningList.Bounds.Bottom + 18.0f;
        }

        foreach (var node in warningList.GetNodes<TodoListEntryNode>()) {
            node.Update();
        }
    }
    
    private void OpenConfig() {
        configWindow?.Dispose();
        configWindow = new TodoOverlayPanelConfigWindow(ModuleTodoOverlayConfig, Config) {
            Size = new Vector2(575.0f, 500.0f),
            InternalName = "TodoListPanelConfig",
            Title = $"{Config.Label} Panel Config",
        };

        configWindow.Toggle();
    }
    
    private TodoListEntryNode BuildTodoEntry(ModuleBase data) {
        var newNode = new TodoListEntryNode {
            Height = 24.0f,
            TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge,
            AlignmentType = AlignmentType.Left,
            Module = data,
            String = data.Name,
            Config = Config,
        };

        if (data.Tooltip is { TooltipText.IsEmpty: false } ) {
            newNode.TextTooltip = data.Tooltip.TooltipText;

            if (data.Tooltip is { ClickAction: not PayloadId.Unset }) {

            }
        }
        
        return newNode;
    }
}
