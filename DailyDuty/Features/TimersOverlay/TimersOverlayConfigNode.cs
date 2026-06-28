using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlayConfigNode : UpdatableNode {
    private readonly TimersOverlay module;
    private readonly VerticalListNode listNode;
    private readonly ScrollingNode<VerticalListNode> colorEdit;

    public TimersOverlayConfigNode(TimersOverlay module) {
        this.module = module;
        listNode = new VerticalListNode {
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
                    IsChecked = module.ModuleTimersOverlayConfig.HideInDuties,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideInDuties = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_HideInQuests,
                    IsChecked = module.ModuleTimersOverlayConfig.HideInQuestEvents,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideInQuestEvents = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_HideSeconds,
                    IsChecked = module.ModuleTimersOverlayConfig.HideTimerSeconds,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.HideTimerSeconds = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_ShowLabel,
                    IsChecked = module.ModuleTimersOverlayConfig.ShowLabel,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.ShowLabel = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_ShowCountdown,
                    IsChecked = module.ModuleTimersOverlayConfig.ShowCountdownText,
                    OnClick = newValue => {
                        module.ModuleTimersOverlayConfig.ShowCountdownText = newValue;
                        module.ModuleTimersOverlayConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.TimersOverlay_EnableMoving,
                    IsChecked = module.ModuleTimersOverlayConfig.EnableMovingTimers,
                    OnClick = newValue => module.ModuleTimersOverlayConfig.EnableMovingTimers = newValue,
                },
                new ResNode { Height = 1.0f },
                new HorizontalFlexNode {
                    Height = 28.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth | FlexFlags.CenterHorizontally,
                    InitialNodes = [
                        new TextNode {
                            String = Strings.TimersOverlay_Scale,
                            AlignmentType = AlignmentType.Left,
                        },
                        new FloatSliderNode {
                            Min = 0.5f,
                            Max = 5.0f,
                            Value = module.ModuleTimersOverlayConfig.Scale,
                            OnValueChanged = newValue => {
                                module.ModuleTimersOverlayConfig.Scale = newValue;
                                module.ModuleTimersOverlayConfig.MarkDirty();
                            },
                        },
                    ],
                },
            ],
        };

        listNode.RecalculateLayout();
        listNode.AttachNode(this);

        colorEdit = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                ItemSpacing = 4.0f,
                FitWidth = true,
            },
            AutoHideScrollBar = true,
        };
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
        colorEdit.ContentNode.Clear();

        foreach (var name in module.ModuleTimersOverlayConfig.EnabledTimers) {
            var config = module.ModuleTimersOverlayConfig.TimerData[name];

            var originalColor = config.Color;
            colorEdit.ContentNode.AddNode(new ColorEditNode {
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

        colorEdit.RecalculateSizes();
    }
}
