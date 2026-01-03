using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TimersOverlay;

public class ConfigNode : UpdatableNode {
    private readonly TimersOverlay module;
    private readonly VerticalListNode listNode;
    private readonly ScrollingListNode colorEdit;
    
    public ConfigNode(TimersOverlay module) {
        this.module = module;
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    Label= "Feature Configuration",
                    Alignment = AlignmentType.Bottom,
                },
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
                    IsChecked = module.ModuleConfig.HideInQuestEvents,
                    OnClick = newValue => {
                        module.ModuleConfig.HideInQuestEvents = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Hide Timer Seconds",
                    IsChecked = module.ModuleConfig.HideTimerSeconds,
                    OnClick = newValue => {
                        module.ModuleConfig.HideTimerSeconds = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Show Module Label",
                    IsChecked = module.ModuleConfig.ShowLabel,
                    OnClick = newValue => {
                        module.ModuleConfig.ShowLabel = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Show Countdown Text",
                    IsChecked = module.ModuleConfig.ShowCountdownText,
                    OnClick = newValue => {
                        module.ModuleConfig.ShowCountdownText = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Enable Moving Timers",
                    IsChecked = module.ModuleConfig.EnableMovingTimers,
                    OnClick = newValue => module.ModuleConfig.EnableMovingTimers = newValue,
                },
                new ResNode{ Height = 1.0f },
                new HorizontalFlexNode {
                    Height = 28.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth | FlexFlags.CenterHorizontally,
                    InitialNodes = [
                        new TextNode {
                            String = "Scale",
                            AlignmentType = AlignmentType.Left,
                        },
                        new SliderNode {
                            Range = 50..500,
                            DecimalPlaces = 2,
                            Value = (int)(module.ModuleConfig.Scale * 100),
                            OnValueChanged = newValue => {
                                module.ModuleConfig.Scale = newValue / 100.0f;
                                module.ModuleConfig.MarkDirty();
                            },
                        },
                    ],
                },
            ],
        };
        
        listNode.RecalculateLayout();
        listNode.AttachNode(this);

        colorEdit = new ScrollingListNode {
            ItemSpacing = 4.0f,
            AutoHideScrollBar = true,
        };
        
        colorEdit.FitWidth = true;
        colorEdit.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        listNode.Size = new Vector2(Width, listNode.Nodes.Sum(node => node.IsVisible ? node.Height + listNode.ItemSpacing : 0.0f));
        listNode.RecalculateLayout();
        
        colorEdit.Size = new Vector2(Width, Height - listNode.Height - 4.0f);
        colorEdit.Position = new Vector2(0.0f, listNode.Bounds.Bottom + 4.0f);
    }

    private int? lastModuleCount;
    
    public override void Update() {
        if (lastModuleCount is null || module.ModuleConfig.EnabledTimers.Count != lastModuleCount) {
            RebuildOptionsList();
            lastModuleCount = module.ModuleConfig.EnabledTimers.Count;
        }
    }

    private void RebuildOptionsList() {
        colorEdit.Clear();

        foreach (var name in module.ModuleConfig.EnabledTimers) {
            var config = module.ModuleConfig.TimerData[name];
            
            var originalColor = config.Color;
            colorEdit.AddNode(new ColorEditNode {
                Height = 28.0f,
                Label = name,
                CurrentColor = config.Color,
                DefaultColor = KnownColor.CornflowerBlue.Vector(),
                OnColorPreviewed = color => {
                    config.Color = color;
                },
                OnColorCancelled = () => {
                    config.Color = originalColor;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorConfirmed = color => {
                    config.Color = color;
                    module.ModuleConfig.MarkDirty();
                },
            });
        }
        
        colorEdit.RecalculateLayout();
    }
}
