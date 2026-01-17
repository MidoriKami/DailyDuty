using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Color;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlayPanelConfigWindow(TodoOverlayConfig moduleTodoOverlayConfig, TodoPanelConfig config, TextNode? labelTextNode = null) : NativeAddon {

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        VerticalListNode listNode;

        var originalTextColor = config.TextColor;
        var originalOutlineColor = config.OutlineColor;
        HorizontalFlexNode flexNode;
        
        AddNode(flexNode = new HorizontalFlexNode {
            AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
            Position = ContentStartPosition,
            Size = ContentSize,
        });
        
        flexNode.AddNode(listNode = new VerticalListNode {
            FitWidth = true,
            FitContents = true,
            ItemSpacing = 4.0f,
            Width = ContentSize.X / 2.0f,
            InitialNodes = [
                new TextInputNode {
                    Height = 28.0f,
                    String = config.Label,
                    OnInputReceived = input => {
                        config.Label = input.ToString();
                        WindowNode?.SetTitle($"{config.Label} Panel Config");
                        labelTextNode?.String = config.Label;
                        moduleTodoOverlayConfig.MarkDirty();
                    },
                },
                new HorizontalFlexNode {
                    Height = 28.0f,
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
                                moduleTodoOverlayConfig.MarkDirty();
                            },
                        },
                    ],
                },
                new HorizontalFlexNode {
                    Height = 28.0f,
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
                                moduleTodoOverlayConfig.MarkDirty();
                            },
                        },
                    ],
                },
                new HorizontalFlexNode {
                    Height = 28.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            String = "Background Alpha",
                            AlignmentType = AlignmentType.Left,
                        },
                        new SliderNode {
                            Range = 10..100,
                            Value = (int) ( config.Alpha * 100 ),
                            OnValueChanged = newValue => {
                                config.Alpha = newValue / 100.0f;
                                moduleTodoOverlayConfig.MarkDirty();
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
                        moduleTodoOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    String = "Enable Moving",
                    Height = 28.0f,
                    IsChecked = config.EnableMoving,
                    OnClick = newValue => config.EnableMoving = newValue,
                },
                new CheckboxNode {
                    String = "Pin to Quest List",
                    Height = 28.0f,
                    IsChecked = config.AttachToQuestList,
                    OnClick = newValue => {
                        config.AttachToQuestList = newValue;
                        moduleTodoOverlayConfig.MarkDirty();
                    },
                },
                new ColorEditNode {
                    Height = 28.0f,
                    String = "Text Color",
                    DefaultColor = ColorHelper.GetColor(1),
                     CurrentColor = config.TextColor,
                     OnColorPreviewed = color => {
                         config.TextColor = color;
                     },
                     OnColorCancelled = () => {
                         config.TextColor = originalTextColor;
                         moduleTodoOverlayConfig.MarkDirty();
                     },
                     OnColorConfirmed = color => {
                         config.TextColor = color;
                         moduleTodoOverlayConfig.MarkDirty();
                     },
                },
                new ColorEditNode {
                    Height = 28.0f,
                    String = "Text Outline Color",
                    DefaultColor = ColorHelper.GetColor(53),
                    CurrentColor = config.OutlineColor,
                    OnColorPreviewed = color => {
                        config.OutlineColor = color;
                    },
                    OnColorCancelled = () => {
                        config.OutlineColor = originalOutlineColor;
                        moduleTodoOverlayConfig.MarkDirty();
                    },
                    OnColorConfirmed = color => {
                        config.OutlineColor = color;
                        moduleTodoOverlayConfig.MarkDirty();
                    },
                },
            ],
        });
        
        listNode.RecalculateLayout();

        ScrollingListNode scrollingList;
        
        var verticalListNode = new VerticalListNode {
            FitWidth =  true,
            ItemSpacing = 4.0f,
            Width = ContentSize.X / 2.0f,
            InitialNodes = [
                scrollingList = new ScrollingListNode {
                    ItemSpacing = 4.0f,
                    AutoHideScrollBar = true,
                    Size = new Vector2(ContentSize.X / 2.0f, ContentSize.Y),
                },
            ],
        };
        flexNode.AddNode(verticalListNode);

        foreach (var module in ModuleManager.GetModules()) {
            scrollingList.AddNode(new CheckboxNode {
                Height = 28.0f,
                String = module.Name,
                IsChecked = config.Modules.Contains(module.Name),
                IsEnabled = module.State is LoadedState.Enabled,
                OnClick = newValue => {
                    if (newValue) {
                        config.Modules.Add(module.Name);
                    }
                    else {
                        config.Modules.Remove(module.Name);
                    }
                    moduleTodoOverlayConfig.MarkDirty();
                },
            });
        }
        
        scrollingList.RecalculateLayout();
    }
}
