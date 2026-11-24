using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UI;
using KamiToolKit.UI.ContextMenu;
using KamiToolKit.Core.Enums;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;
using KamiToolKit.UI.Overlays.UiOverlay;

namespace DailyDuty.Nodes.Overlay;

public sealed class TodoPanelNode : OverlayNode {

    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

    private readonly WindowFrameNode windowFrameNode;
    private readonly TextNode titleTextNode;
    private readonly CircleButtonNode optionsButtonNode;
    private readonly HorizontalLineNode horizontalLineNode;
    private readonly VerticalListNode moduleListNode;
    private readonly TextNode noModulesTextNode;

    private readonly Dictionary<IModule, TextNode> entryNodes = new();
    private readonly Dictionary<IModule, bool> previousVisibility = new();

    private readonly ContextMenu optionsMenu = new();

    private TodoPanelConfig PanelConfig { get; }
    private TodoOverlayConfig Config { get; }

    public unsafe TodoPanelNode(TodoPanelConfig panelConfig, TodoOverlayConfig config) {
        PanelConfig = panelConfig;
        Config = config;

        Position = panelConfig.Position ?? (Vector2)AtkStage.Instance()->ScreenSize / 2.0f;

        windowFrameNode = new WindowFrameNode {
            IsVisible = panelConfig.ShowFrame,
            Alpha = panelConfig.Alpha,
        };
        windowFrameNode.AttachNode(this);

        titleTextNode = new TextNode {
            FontType = FontType.TrumpGothic,
            TextColor = ColorHelper.GetColor(2),
            FontSize = 23,
            String = panelConfig.Label,
        };
        titleTextNode.AttachNode(this);

        optionsButtonNode = new CircleButtonNode {
            Icon = CircleButtonIcon.GearCog,
            OnClick = ShowOptionsContextMenu,
            Alpha = MathF.Max(0.1f, panelConfig.ButtonAlpha),
        };
        optionsButtonNode.AttachNode(this);

        horizontalLineNode = new HorizontalLineNode();
        horizontalLineNode.AttachNode(this);

        moduleListNode = new VerticalListNode {
            FitContents = true,
            ItemSpacing = panelConfig.ItemSpacing,
            StretchWidth = true,
        };
        moduleListNode.AttachNode(this);

        noModulesTextNode = new TextNode {
            AlignmentType = AlignmentType.Center,
            FontSize = 14,
            TextFlags = TextFlags.Glare,
            TextColor = ColorHelper.GetColor(1),
            TextOutlineColor = ColorHelper.GetColor(32),
            String = "No action required",
        };
        noModulesTextNode.AttachNode(this);

        foreach (var module in System.Controller.Modules) {
            var textNode = new TextNode {
                Height = 24.0f,
                TextFlags = TextFlags.Ellipsis | TextFlags.Edge,
                TextColor = panelConfig.TextColor,
                TextOutlineColor = panelConfig.OutlineColor,
                AlignmentType = panelConfig.Alignment == VerticalListAlignment.Right ? AlignmentType.Right : AlignmentType.Left,
                String = module.DisplayName,
            };
            moduleListNode.AddNode(textNode);
            entryNodes[module] = textNode;

            switch (module) {
                case IClickableTooltip clickableTooltip:
                    textNode.TextTooltip = clickableTooltip.GetTooltip();
                    textNode.ShowClickableCursor = true;
                    textNode.AddEvent(AtkEventType.MouseClick, clickableTooltip.OnTooltipClicked);
                    break;

                case ITooltip tooltip:
                    textNode.TextTooltip = tooltip.GetTooltip();
                    textNode.ShowClickableCursor = true;
                    break;
            }
        }

        OnEditComplete = _ => {
            if (PanelConfig.AttachToQuestList) return;
            PanelConfig.Position = Position;
            config.Save();
        };
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        windowFrameNode.Size = Size;

        titleTextNode.Size = new Vector2(Width, 31.0f);
        titleTextNode.Position = new Vector2(12.0f, 7.0f);

        optionsButtonNode.Size = new Vector2(28.0f, 28.0f);
        optionsButtonNode.Position = new Vector2(Width - optionsButtonNode.Width - 6.0f, 7.0f);

        horizontalLineNode.Size = new Vector2(Width - 12.0f, 4.0f);
        horizontalLineNode.Position = new Vector2(6.0f, titleTextNode.Bounds.Bottom - 4.0f);

        noModulesTextNode.Size = new Vector2(Width - 48.0f, 24.0f);
        noModulesTextNode.Position = new Vector2(18.0f, horizontalLineNode.Bounds.Bottom + 6.0f + (Height - horizontalLineNode.Bounds.Bottom - 20.0f - 24.0f) / 2.0f);

        moduleListNode.Size = new Vector2(Width - 48.0f, Height - horizontalLineNode.Bounds.Bottom - 20.0f);
        moduleListNode.Position = new Vector2(PanelConfig.ListXOffset, horizontalLineNode.Bounds.Bottom + 6.0f);
    }

    protected override unsafe void OnUpdate() {
        // Pin to quest list if enabled
        if (PanelConfig.AttachToQuestList) {
            var todoAddon = new AtkUnitBasePtr((nint)RaptureAtkUnitManager.Instance()->GetAddonByName("_ToDoList"));
            if (!todoAddon.IsNull) {
                Position = todoAddon.Position + todoAddon.ScaledSize - new Vector2(Width / Scale.X, 0.0f);
            }
        }

        // Check for dirty config and apply all style changes
        if (PanelConfig.IsDirty) {
            PanelConfig.IsDirty = false;
            var textAlignment = PanelConfig.Alignment == VerticalListAlignment.Right ? AlignmentType.Right : AlignmentType.Left;
            foreach (var textNode in entryNodes.Values) {
                textNode.TextColor = PanelConfig.TextColor;
                textNode.TextOutlineColor = PanelConfig.OutlineColor;
                textNode.AlignmentType = textAlignment;
            }
            windowFrameNode.Alpha = PanelConfig.Alpha;
            optionsButtonNode.Alpha = MathF.Max(0.1f, PanelConfig.ButtonAlpha);
            Scale = new Vector2(PanelConfig.Scale, PanelConfig.Scale);
            moduleListNode.ItemSpacing = PanelConfig.ItemSpacing;
            windowFrameNode.IsVisible = PanelConfig.ShowFrame;
            titleTextNode.String = PanelConfig.Label;

            if (Math.Abs(Width - PanelConfig.Width) > 0.5f) {
                Size = new Vector2(PanelConfig.Width, Height);
            }

            moduleListNode.Position = new Vector2(PanelConfig.ListXOffset, horizontalLineNode.Bounds.Bottom + 6.0f);
        }

        var changed = false;

        foreach (var (module, textNode) in entryNodes) {
            var visible = module.Enabled
                          && module.LoadState is State.Enabled
                          && PanelConfig.Modules.Contains(module.DisplayName)
                          && module.GetStatus().RequiresAction;

            if (previousVisibility.TryGetValue(module, out var prev) && prev == visible) {
                continue;
            }

            previousVisibility[module] = visible;
            textNode.IsVisible = visible;
            changed = true;
        }

        if (changed) {
            moduleListNode.UpdateLayout();

            var anyVisible = previousVisibility.Values.Any(v => v);
            noModulesTextNode.IsVisible = !anyVisible;

            if (PanelConfig.AutoHideWhenEmpty) {
                IsVisible = anyVisible;
            }
        }

        EnableMoving = PanelConfig.EnableMoving;

        var desiredHeight = MathF.Max(96.0f, horizontalLineNode.Bounds.Bottom + moduleListNode.Height + 27.0f);
        if (Math.Abs(Height - desiredHeight) > 0.5f) {
            Size = new Vector2(Width, desiredHeight);
        }
    }

    public void Show() {
        IsVisible = true;
    }

    public void Hide() {
        IsVisible = false;
    }

    public bool HasVisibleItems() {
        return previousVisibility.Values.Any(v => v);
    }

    private void ShowOptionsContextMenu() {
        optionsMenu.Clear();

        optionsMenu.AddItem(new ContextMenuItem {
            Name = PanelConfig.ShowFrame ? "Hide Background" : "Show Background",
            OnClick = () => {
                PanelConfig.ShowFrame = !PanelConfig.ShowFrame;
                windowFrameNode.IsVisible = PanelConfig.ShowFrame;
                Config.Save();
            },
        });

        optionsMenu.AddItem(new ContextMenuItem {
            Name = "Open Config Window",
            OnClick = System.ConfigWindow.Toggle,
        });

        optionsMenu.Open();
    }
}
