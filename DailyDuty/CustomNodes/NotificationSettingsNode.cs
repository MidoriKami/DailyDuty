using Resources;
using System;
using System.Linq;
using DailyDuty.Classes;
using Dalamud.Game.Text;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using XivChatTypeExtensions = DailyDuty.Extensions.XivChatTypeExtensions;

namespace DailyDuty.CustomNodes;

public class NotificationSettingsNode<T> : SimpleComponentNode where T : ModuleBase {
    private readonly TabbedVerticalListNode listNode;

    public NotificationSettingsNode(T module) {
        listNode = new TabbedVerticalListNode {
            FitWidth = true,
            ItemVerticalSpacing = 4.0f,
        };
        listNode.AttachNode(this);

        listNode.AddNode([
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Notification Settings", Strings.Culture) ?? "Notification Settings",
            },
            new CheckboxNode {
                String = Strings.ResourceManager.GetString("Send status on login", Strings.Culture) ?? "Send status on login",
                Height = 24.0f,
                IsChecked = module.ConfigBase.OnLoginMessage,
                OnClick = value => {
                    module.ConfigBase.OnLoginMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CheckboxNode {
                String = Strings.ResourceManager.GetString("Send status on zone change", Strings.Culture) ?? "Send status on zone change",
                Height = 24.0f,
                IsChecked = module.ConfigBase.OnZoneChangeMessage,
                OnClick = value => {
                    module.ConfigBase.OnZoneChangeMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CheckboxNode {
                String = Strings.ResourceManager.GetString("Send status on module reset", Strings.Culture) ?? "Send status on module reset",
                Height = 24.0f,
                IsChecked = module.ConfigBase.ResetMessage,
                OnClick = value => {
                    module.ConfigBase.ResetMessage = value;
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Custom Status Message", Strings.Culture) ?? "Custom Status Message",
            },
            new TextInputNode {
                Height = 28.0f,
                String = module.ConfigBase.CustomStatusMessage,
                PlaceholderString = Strings.ResourceManager.GetString("Custom Status Message", Strings.Culture) ?? "Custom Status Message",
                OnInputReceived = value => {
                    module.ConfigBase.CustomStatusMessage = value.ToString();
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Custom Reset Message", Strings.Culture) ?? "Custom Reset Message",
            },
            new TextInputNode {
                PlaceholderString = Strings.ResourceManager.GetString("Custom Reset Message", Strings.Culture) ?? "Custom Reset Message",
                Height = 28.0f,
                String = module.ConfigBase.CustomResetMessage,
                OnInputReceived = value => {
                    module.ConfigBase.CustomResetMessage = value.ToString();
                    module.ConfigBase.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                String = Strings.ResourceManager.GetString("Chat Channel", Strings.Culture) ?? "Chat Channel",
            },
            new TextDropDownNode {
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
