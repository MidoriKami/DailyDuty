using Resources;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBarConfigNode : SimpleComponentNode {
    private readonly VerticalListNode listNode;

    public ServerInfoBarConfigNode(ServerInfoBar module) {
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.ResourceManager.GetString("Feature Configuration", Strings.Culture) ?? "Feature Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Hide Seconds", Strings.Culture) ?? "Hide Seconds",
                    IsChecked = module.ModuleConfig.HideSeconds,
                    OnClick = newValue => {
                        module.ModuleConfig.HideSeconds = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Toggleable Timer", Strings.Culture) ?? "Toggleable Timer",
                    IsChecked = module.ModuleConfig.Combo,
                    OnClick = newValue => {
                        module.ModuleConfig.Combo = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Daily Timer", Strings.Culture) ?? "Daily Timer",
                    IsChecked = module.ModuleConfig.SoloDaily,
                    OnClick = newValue => {
                        module.ModuleConfig.SoloDaily = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Weekly Timer", Strings.Culture) ?? "Weekly Timer",
                    IsChecked = module.ModuleConfig.SoloWeekly,
                    OnClick = newValue => {
                        module.ModuleConfig.SoloWeekly = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
            ],
        };

        listNode.RecalculateLayout();
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
