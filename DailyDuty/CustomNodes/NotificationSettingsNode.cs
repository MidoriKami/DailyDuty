using System;
using System.Linq;
using DailyDuty.Classes;
using Dalamud.Game.Text;
using KamiToolKit.Nodes;
using XivChatTypeExtensions = DailyDuty.Extensions.XivChatTypeExtensions;

namespace DailyDuty.CustomNodes;

public class NotificationSettingsNode<T> : ResNode where T : ModuleBase {
    private readonly TabbedVerticalListNode listNode;

    public NotificationSettingsNode(T module) {
        listNode = new TabbedVerticalListNode {
            FitWidth = true,
            ItemSpacing = 4.0f,
        };
        listNode.AttachNode(this);

        listNode.AddNode([
            new CategoryHeaderNode {
                String = Strings.NotificationSettingsNode_NotificationSettings,
            },
            new CheckboxNode {
                String = Strings.NotificationSettingsNode_LoginStatus,
                Height = 24.0f,
                IsChecked = module.ConfigBase.OnLoginMessage,
                OnClick = value => {
                    module.ConfigBase.OnLoginMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CheckboxNode {
                String = Strings.NotificationSettingsNode_ZoneStatus,
                Height = 24.0f,
                IsChecked = module.ConfigBase.OnZoneChangeMessage,
                OnClick = value => {
                    module.ConfigBase.OnZoneChangeMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CheckboxNode {
                String = Strings.NotificationSettingsNode_ResetStatus,
                Height = 24.0f,
                IsChecked = module.ConfigBase.ResetMessage,
                OnClick = value => {
                    module.ConfigBase.ResetMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.NotificationSettingsNode_CustomStatus,
            },
            new TextInputNode {
                Height = 28.0f,
                String = module.ConfigBase.CustomStatusMessage,
                PlaceholderString = Strings.NotificationSettingsNode_CustomStatus,
                OnInputReceived = value => {
                    module.ConfigBase.CustomStatusMessage = value.ToString();
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.NotificationSettingsNode_CustomReset,
            },
            new TextInputNode {
                PlaceholderString = Strings.NotificationSettingsNode_CustomReset,
                Height = 28.0f,
                String = module.ConfigBase.CustomResetMessage,
                OnInputReceived = value => {
                    module.ConfigBase.CustomResetMessage = value.ToString();
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.NotificationSettingsNode_ChatChannel,
            },
            new StringDropDownNode {
                Height = 24.0f,
                Options = Enum.GetValues<XivChatType>().Select(chatType => chatType.Description).ToList(),
                SelectedOption = module.ConfigBase.MessageChatChannel.Description,
                OnOptionSelected = newValue => {
                    module.ConfigBase.MessageChatChannel = XivChatTypeExtensions.Parse(newValue);
                    module.ConfigBase.MarkDirty();
                },
                MaxListOptions = 20,
            },
        ]);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
