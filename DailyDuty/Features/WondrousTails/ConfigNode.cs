using System.Drawing;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;

namespace DailyDuty.Features.WondrousTails;

public class ConfigNode(WondrousTails module) : ConfigNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;

    protected override void BuildNode(ScrollingListNode container) {
        var originalColor = module.ModuleConfig.DutyFinderColor;
        
        container.AddNode([
            new CheckboxNode {
                Height = 28.0f,
                String = "Instance Notifications",
                IsChecked = module.ModuleConfig.InstanceNotifications,
                OnClick = newValue => {
                    module.ModuleConfig.InstanceNotifications = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Sticker Available Notification",
                IsChecked = module.ModuleConfig.StickerAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.StickerAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Unclaimed Book Notification",
                IsChecked = module.ModuleConfig.UnclaimedBookWarning,
                OnClick = newValue => {
                    module.ModuleConfig.UnclaimedBookWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Shuffle Available Notification",
                IsChecked = module.ModuleConfig.ShuffleAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.ShuffleAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                Label = "Duty Finder Integration",
            },
            new CheckboxNode {
                Height = 28.0f,
                String = "Recolor Duty Finder Entries",
                IsChecked = module.ModuleConfig.ColorDutyFinderText,
                OnClick = newValue => {
                    module.ModuleConfig.ColorDutyFinderText = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new ColorEditNode {
                Height = 28.0f,
                CurrentColor = module.ModuleConfig.DutyFinderColor,
                Label = "Entry Color",
                DefaultColor = KnownColor.Yellow.Vector(),
                OnColorPreviewed = color => {
                    module.ModuleConfig.DutyFinderColor = color;
                },
                OnColorCancelled = () => {
                    module.ModuleConfig.DutyFinderColor = originalColor;
                    module.ModuleConfig.MarkDirty();
                },
                OnColorConfirmed = color => {
                    module.ModuleConfig.DutyFinderColor = color;
                    module.ModuleConfig.MarkDirty();
                }
            },
            new ResNode{ Height = 4.0f },
            new CheckboxNode {
                Height = 28.0f,
                String = "Show Clover Indicator",
                IsChecked = module.ModuleConfig.CloverIndicator,
                OnClick = newValue => {
                    module.ModuleConfig.CloverIndicator = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ]);
    }
}
