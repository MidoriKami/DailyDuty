using System.Drawing;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.WondrousTails;

public class WondrousTailsConfigNode(WondrousTails module) : ConfigNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;

    protected override void BuildNode(VerticalListNode container) {
        var originalColor = module.ModuleConfig.DutyFinderColor;

        container.AddNode([
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.WondrousTails_InstanceNotifications,
                IsChecked = module.ModuleConfig.InstanceNotifications,
                OnClick = newValue => {
                    module.ModuleConfig.InstanceNotifications = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.WondrousTails_StickerNotification,
                IsChecked = module.ModuleConfig.StickerAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.StickerAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.WondrousTails_UnclaimedNotification,
                IsChecked = module.ModuleConfig.UnclaimedBookWarning,
                OnClick = newValue => {
                    module.ModuleConfig.UnclaimedBookWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.WondrousTails_ShuffleNotification,
                IsChecked = module.ModuleConfig.ShuffleAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.ShuffleAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.WondrousTails_DutyFinderIntegration,
            },
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.WondrousTails_RecolorEntries,
                IsChecked = module.ModuleConfig.ColorDutyFinderText,
                OnClick = newValue => {
                    module.ModuleConfig.ColorDutyFinderText = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new ColorEditNode {
                Height = 28.0f,
                CurrentColor = module.ModuleConfig.DutyFinderColor,
                String = Strings.WondrousTails_EntryColor,
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
                String = Strings.WondrousTails_ShowClover,
                IsChecked = module.ModuleConfig.CloverIndicator,
                OnClick = newValue => {
                    module.ModuleConfig.CloverIndicator = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ]);
    }
}
