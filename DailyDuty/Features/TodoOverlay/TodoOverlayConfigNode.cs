using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlayConfigNode : SimpleComponentNode {
    private readonly TodoOverlay module;

    private readonly VerticalListNode configNode;
    private readonly ScrollingNode<VerticalListNode> listNode;
    private TodoOverlayPanelConfigWindow? panelConfigWindow;

    public TodoOverlayConfigNode(TodoOverlay module) {
        this.module = module;

        configNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.ServerInfoBar_FeatureConfig,
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_HideInDuties,
                    IsChecked = module.ModuleTodoOverlayConfig.HideInDuties,
                    OnClick = newValue => {
                        module.ModuleTodoOverlayConfig.HideInDuties = newValue;
                        module.ModuleTodoOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_HideInQuests,
                    IsChecked = module.ModuleTodoOverlayConfig.HideDuringQuests,
                    OnClick = newValue => {
                        module.ModuleTodoOverlayConfig.HideDuringQuests = newValue;
                        module.ModuleTodoOverlayConfig.MarkDirty();
                    },
                },
                new CategoryHeaderNode {
                    String = Strings.TodoOverlay_Panels,
                    Alignment = AlignmentType.Bottom,
                },
                listNode = new ScrollingNode<VerticalListNode> {
                    ContentNode = {
                        FitWidth = true,
                        ItemSpacing = 8.0f,
                    },
                },
            ],
        };
        configNode.AttachNode(this);

        RebuildList();
    }

    private void RebuildList() {
        listNode.ContentNode.Clear();

        foreach (var panel in module.ModuleTodoOverlayConfig.Panels) {
            NodeBase entry;
            CircleButtonNode removeButton;
            TextNode labelTextNode;

            listNode.ContentNode.AddNode([
                entry = new HorizontalListNode {
                    FitHeight = true,
                    Height = 32.0f,
                    ItemSpacing = 6.0f,
                    InitialNodes = [
                        removeButton = new CircleButtonNode {
                            Size = new Vector2(32.0f, 32.0f),
                            Icon = CircleButtonIcon.Cross,
                        },
                        labelTextNode = new TextNode {
                            Size = new Vector2(300.0f, 32.0f),
                            String = panel.Label,
                            AlignmentType = AlignmentType.Left,
                        },
                        new CircleButtonNode {
                            Size = new Vector2(32.0f, 32.0f),
                            Icon = CircleButtonIcon.Edit,
                            OnClick = () => {
                                panelConfigWindow?.Dispose();
                                panelConfigWindow = new TodoOverlayPanelConfigWindow(module.ModuleTodoOverlayConfig, panel, labelTextNode) {
                                    Size = new Vector2(575.0f, 500.0f),
                                    InternalName = "TodoListPanelConfig",
                                    Title = $"{panel.Label} {Strings.PanelConfig_Config}",
                                };

                                panelConfigWindow.Toggle();
                            },
                        },
                    ],
                },
            ]);

            removeButton.OnClick = () => {
                module.ModuleTodoOverlayConfig.Panels.Remove(panel);
                module.ModuleTodoOverlayConfig.MarkDirty();
                module.RebuildPanels();
                listNode.ContentNode.RemoveNode(entry);
                listNode.RecalculateSizes();
            };
        }

        listNode.ContentNode.AddNode(new HorizontalListNode {
            FitHeight = true,
            Height = 32.0f,
            ItemSpacing = 6.0f,
            InitialNodes = [
                new CircleButtonNode {
                    Size = new Vector2(32.0f, 32.0f),
                    Icon = CircleButtonIcon.Add,
                    OnClick = () => {
                        module.ModuleTodoOverlayConfig.Panels.Add(new TodoPanelConfig {
                            EnableMoving = true,
                        });
                        module.ModuleTodoOverlayConfig.MarkDirty();
                        module.RebuildPanels();
                        RebuildList();
                    },
                },
                new TextNode {
                    Size = new Vector2(300.0f, 32.0f),
                    String = Strings.TodoOverlay_AddPanel,
                    AlignmentType = AlignmentType.Left,
                },
            ],
        });

        listNode.RecalculateSizes();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        const float featureConfigSize = 150.0f;

        configNode.Size = new Vector2(Width, featureConfigSize);
        listNode.Size = new Vector2(Width, Height - featureConfigSize - 48.0f);

        configNode.RecalculateLayout();
        listNode.RecalculateSizes();
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        base.Dispose(disposing, isNativeDestructor);

        panelConfigWindow?.Dispose();
        panelConfigWindow = null;
    }
}
