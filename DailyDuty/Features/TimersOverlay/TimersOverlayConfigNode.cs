using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Color;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlayConfigNode : UpdatableNode {
    private readonly TimersOverlay module;
    private readonly VerticalListNode listNode;
    private readonly ScrollingListNode colorEdit;
    
    public TimersOverlayConfigNode(TimersOverlay module) {
        this.module = module;
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String= "Feature Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Hide in Duties",
                    IsChecked = module.ModuleTimersOverlayConfig.HideInDuties,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideInDuties = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Hide in Quest Events",
                    IsChecked = module.ModuleTimersOverlayConfig.HideInQuestEvents,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideInQuestEvents = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Hide Timer Seconds",
                    IsChecked = module.ModuleTimersOverlayConfig.HideTimerSeconds,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideTimerSeconds = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Show Module Label",
                    IsChecked = module.ModuleTimersOverlayConfig.ShowLabel,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.ShowLabel = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Show Countdown Text",
                    IsChecked = module.ModuleTimersOverlayConfig.ShowCountdownText,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.ShowCountdownText = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Enable Moving Timers",
                    IsChecked = module.ModuleTimersOverlayConfig.EnableMovingTimers,
                    OnClick = newValue => module.ModuleTimersOverlayConfig.EnableMovingTimers = newValue,
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
                            Value = (int)(module.ModuleTimersOverlayConfig.Scale * 100),
                            OnValueChanged = newValue => {
                                module.ModuleTimersOverlayConfig.Scale = newValue / 100.0f;
                                module.ModuleTimersOverlayConfig.MarkDirty();
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
        if (lastModuleCount is null || module.ModuleTimersOverlayConfig.EnabledTimers.Count != lastModuleCount) {
            RebuildOptionsList();
            lastModuleCount = module.ModuleTimersOverlayConfig.EnabledTimers.Count;
        }
    }

    private void RebuildOptionsList() {
        colorEdit.Clear();

        foreach (var name in module.ModuleTimersOverlayConfig.EnabledTimers) {
            var config = module.ModuleTimersOverlayConfig.TimerData[name];
            
            var originalColor = config.Color;
            colorEdit.AddNode(new ColorEditNode {
                Height = 28.0f,
                String = name,
                CurrentColor = config.Color,
                DefaultColor = KnownColor.CornflowerBlue.Vector(),
                OnColorPreviewed = color => {
                    config.Color = color;
                },
                OnColorCancelled = () => {
                    config.Color = originalColor;
                    module.ModuleTimersOverlayConfig.MarkDirty();
                },
                OnColorConfirmed = color => {
                    config.Color = color;
                    module.ModuleTimersOverlayConfig.MarkDirty();
                },
            });
        }
        
        colorEdit.RecalculateLayout();
    }
}
