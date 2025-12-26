using System;
using System.Linq;
using DailyDuty.Extensions;
using Dalamud.Game.Text;
using KamiToolKit.Nodes;
using XivChatTypeExtensions = DailyDuty.Extensions.XivChatTypeExtensions;

namespace DailyDuty.Classes.Nodes;

public class NotificationSettingsNode<T> : SimpleComponentNode where T : ModuleBase {
    private readonly TabbedVerticalListNode listNode;
    
    public NotificationSettingsNode(T module) {
        listNode = new TabbedVerticalListNode {
            FitWidth = true,
            ItemVerticalSpacing = 4.0f,
        };
        listNode.AttachNode(this);
        
        listNode.AddNode(new CategoryHeaderNode {
            Label = "Notification Settings",
        });
        
        listNode.AddNode(new CheckboxNode {
            String = "Send status on login",
            Height = 24.0f,
            IsChecked = module.ConfigBase.OnLoginMessage,
            OnClick = value => {
                module.ConfigBase.OnLoginMessage = value;
                module.ConfigBase.SavePending = true;
            },
        });

        listNode.AddNode(new CheckboxNode {
            String = "Send status on zone change",
            Height = 24.0f,
            IsChecked = module.ConfigBase.OnZoneChangeMessage,
            OnClick = value => {
                module.ConfigBase.OnZoneChangeMessage = value;
                module.ConfigBase.SavePending = true;
            },
        });

        listNode.AddNode(new CheckboxNode {
            String = "Send status on module reset",
            Height = 24.0f,
            IsChecked = module.ConfigBase.ResetMessage,
            OnClick = value => {
                module.ConfigBase.ResetMessage = value;
                module.ConfigBase.SavePending = true;
            },
        });
        
        listNode.AddNode(new CategoryHeaderNode {
            Label = "Custom Status Message",
        });
        
        listNode.AddNode(new TextInputNode {
            Height = 28.0f,
            String = module.ConfigBase.CustomStatusMessage,
            PlaceholderString = "Custom Status Message",
            OnInputReceived = value => {
                module.ConfigBase.CustomStatusMessage = value.ExtractText();
                module.ConfigBase.SavePending = true;
            },
        });
        
        listNode.AddNode(new CategoryHeaderNode {
            Label = "Custom Reset Message",
        });
        
        listNode.AddNode(new TextInputNode {
            PlaceholderString = "Custom Reset Message",
            Height = 28.0f,
            String = module.ConfigBase.CustomResetMessage,
            OnInputReceived = value => {
                module.ConfigBase.CustomResetMessage = value.ExtractText();
                module.ConfigBase.SavePending = true;
            },
        });
        
        listNode.AddNode(new CategoryHeaderNode {
            Label = "Chat Channel",
        });
        
        listNode.AddNode(new TextDropDownNode {
            Height = 24.0f,
            Options = Enum.GetValues<XivChatType>().Select(chatType => chatType.Description).ToList(),
            SelectedOption = module.ConfigBase.MessageChatChannel.Description,
            OnOptionSelected = newValue => {
                module.ConfigBase.MessageChatChannel = XivChatTypeExtensions.Parse(newValue);
                module.ConfigBase.SavePending = true;
            },
            MaxListOptions = 20,
        });
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
