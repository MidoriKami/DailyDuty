using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TodoOverlay;

public class PanelConfigWindow(Config moduleConfig, TodoPanelConfig config, TextNode? labelTextNode = null) : NativeAddon {

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        VerticalListNode listNode;
        
        AddNode(listNode = new VerticalListNode {
            FitWidth = true,
            FitContents = true,
            ItemSpacing = 4.0f,
            Position = ContentStartPosition,
            Size = new Vector2(ContentSize.X, 64.0f),
            InitialNodes = [
                new TextInputNode {
                    Size = new Vector2(ContentSize.X, 28.0f),
                    String = config.Label,
                    OnInputReceived = input => {
                        config.Label = input.ToString();
                        WindowNode?.SetTitle($"{config.Label} Panel Config");
                        labelTextNode?.String = config.Label;
                        moduleConfig.MarkDirty();
                    },
                },
                new HorizontalFlexNode {
                    Size = new Vector2(ContentSize.X, 24.0f),
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            String = "Alignment",
                            AlignmentType = AlignmentType.Left,
                        },
                        new TextDropDownNode {
                            Options = Enum.GetValues<VerticalListAlignment>().Select(alignment => alignment.Description).ToList(),
                            SelectedOption = config.Alignment.Description,
                            OnOptionSelected = option => {
                                config.Alignment = option.Parse(VerticalListAlignment.Left);
                                moduleConfig.MarkDirty();
                            },
                        },
                    ],
                },
                new HorizontalFlexNode {
                    Size = new Vector2(ContentSize.X, 24.0f),
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            String = "Vertical Spacing",
                            AlignmentType = AlignmentType.Left,
                        },
                        new NumericInputNode {
                            Min = 0,
                            Max = 12,
                            Value = config.ItemSpacing,
                            OnValueUpdate = newValue => {
                                config.ItemSpacing = newValue;
                                moduleConfig.MarkDirty();
                            },
                        },
                    ],
                },
                new CheckboxNode {
                    String = "Hide Frame",
                    Height = 28.0f,
                    IsChecked = !config.ShowFrame,
                    OnClick = newValue => {
                        config.ShowFrame = !newValue;
                        moduleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    String = "Enable Moving",
                    Height = 28.0f,
                    IsChecked = config.EnableMoving,
                    OnClick = newValue => config.EnableMoving = newValue,
                },
                new CategoryHeaderNode {
                    Label = "Modules",
                },
            ],
        });
        
        listNode.RecalculateLayout();

        var remainingHeight = ContentSize.Y - listNode.Height - 8.0f;

        var verticalListNode = new ScrollingListNode {
            Position = new Vector2(ContentStartPosition.X, listNode.Bounds.Bottom + 4.0f),
            Size = new Vector2(ContentSize.X, remainingHeight),
            FitWidth =  true,
            ItemSpacing = 4.0f,
            AutoHideScrollBar = true,
        };
        verticalListNode.AttachNode(this);
        
        foreach (var module in ModuleManager.GetModules()) {
            verticalListNode.AddNode(new CheckboxNode {
                Height = 28.0f,
                String = module.Name,
                IsChecked = config.Modules.Contains(module.Name),
                OnClick = newValue => {
                    if (newValue) {
                        config.Modules.Add(module.Name);
                    }
                    else {
                        config.Modules.Remove(module.Name);
                    }
                    moduleConfig.MarkDirty();
                },
            });
        }
        
        verticalListNode.RecalculateLayout();
    }
}
