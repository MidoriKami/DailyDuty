using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TodoOverlay;

public class ConfigNode : SimpleComponentNode {
    private readonly TodoOverlay module;

    private readonly ScrollingListNode listNode;
    private PanelConfigWindow? panelConfigWindow;
    
    public ConfigNode(TodoOverlay module) {
        this.module = module;

        listNode = new ScrollingListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
        };
        listNode.AttachNode(this);

        RebuildList();
    }

    private void RebuildList() {
        
        listNode.Clear();
        
        listNode.AddNode([
            new CategoryHeaderNode {
                Label= "Feature Configuration",
                Alignment = AlignmentType.Bottom,
            },
            new ResNode{ Height = 4.0f },
            new CheckboxNode {
                Height = 28.0f,
                String = "Hide in Duties",
                IsChecked = module.ModuleConfig.HideInDuties,
                OnClick = newValue => {
                    module.ModuleConfig.HideInDuties = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Hide in Quest Events",
                IsChecked = module.ModuleConfig.HideDuringQuests,
                OnClick = newValue => {
                    module.ModuleConfig.HideDuringQuests = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new ResNode{ Height = 4.0f },
            new CategoryHeaderNode {
                Label= "Overlay Panel Configuration",
                Alignment = AlignmentType.Bottom,
            },
        ]);

        foreach (var panel in module.ModuleConfig.Panels) {
            NodeBase entry;
            CircleButtonNode removeButton;
            TextNode labelTextNode;
            
            listNode.AddNode([
                entry = new HorizontalListNode {
                    FitHeight = true,
                    Height = 32.0f,
                    ItemSpacing = 6.0f,
                    InitialNodes = [
                        removeButton = new CircleButtonNode {
                            Size = new Vector2(32.0f, 32.0f),
                            Icon = ButtonIcon.Cross,
                        },
                        labelTextNode = new TextNode {
                            Size = new Vector2(300.0f, 32.0f),
                            String = panel.Label,
                            AlignmentType = AlignmentType.Left,
                        },
                        new CircleButtonNode {
                            Size = new Vector2(32.0f, 32.0f),
                            Icon = ButtonIcon.Edit,
                            OnClick = () => {
                                panelConfigWindow?.Dispose();
                                panelConfigWindow = new PanelConfigWindow(module.ModuleConfig, panel, labelTextNode) {
                                    InternalName = "TodoListPanelConfig",
                                    Title = $"{panel.Label} Panel Config",
                                };

                                panelConfigWindow.Toggle();
                            },
                        },
                    ],
                },
            ]);
            
            removeButton.OnClick = () => {
                module.ModuleConfig.Panels.Remove(panel);
                module.ModuleConfig.MarkDirty();
                module.RebuildPanels();
                listNode.RemoveNode(entry);
            };
        }

        listNode.AddNode(new HorizontalListNode {
            FitHeight = true,
            Height = 32.0f,
            ItemSpacing = 6.0f,
            InitialNodes = [
                new CircleButtonNode {
                    Size = new Vector2(32.0f, 32.0f),
                    Icon = ButtonIcon.Add,
                    OnClick = () => {
                        module.ModuleConfig.Panels.Add(new TodoPanelConfig());
                        module.ModuleConfig.MarkDirty();
                        module.RebuildPanels();
                        RebuildList();
                    },
                },
                new TextNode {
                    Size = new Vector2(300.0f, 32.0f),
                    String = "Add Panel",
                    AlignmentType = AlignmentType.Left,
                },
            ],
        });
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
    }
}
