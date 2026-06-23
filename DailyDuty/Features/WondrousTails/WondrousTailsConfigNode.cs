using System.Drawing;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Color;

namespace DailyDuty.Features.WondrousTails;

public class WondrousTailsConfigNode(WondrousTails module) : ConfigNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;

    protected override void BuildNode(ScrollingListNode container) {
        var originalColor = module.ModuleConfig.DutyFinderColor;

        container.AddNode([
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Instance_Notifications,
                IsChecked = module.ModuleConfig.InstanceNotifications,
                OnClick = newValue => {
                    module.ModuleConfig.InstanceNotifications = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Sticker_Available_Notification,
                IsChecked = module.ModuleConfig.StickerAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.StickerAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Unclaimed_Book_Notification,
                IsChecked = module.ModuleConfig.UnclaimedBookWarning,
                OnClick = newValue => {
                    module.ModuleConfig.UnclaimedBookWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Shuffle_Available_Notification,
                IsChecked = module.ModuleConfig.ShuffleAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.ShuffleAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.Duty_Finder_Integration,
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Recolor_Duty_Finder_Entries,
                IsChecked = module.ModuleConfig.ColorDutyFinderText,
                OnClick = newValue => {
                    module.ModuleConfig.ColorDutyFinderText = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new ColorEditNode {
                Height = 28.0f,
                CurrentColor = module.ModuleConfig.DutyFinderColor,
                String = Strings.Entry_Color,
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
            new ResNode { Height = 4.0f },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.Show_Clover_Indicator,
                IsChecked = module.ModuleConfig.CloverIndicator,
                OnClick = newValue => {
                    module.ModuleConfig.CloverIndicator = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ]);
    }
}
