using System;
using Dalamud.Game.Text;
using KamiLib.Configuration;

namespace DailyDuty.Models.Settings;

[Serializable]
public class MessageSettings
{
    public Setting<XivChatType> MessageChatChannel = new(Service.PluginInterface.GeneralChatType);
    public Setting<bool> UseCustomChannel = new(false);
    public Setting<bool> LinkEnabled = new(false);
    public Setting<bool> OnLoginMessage = new(false);
    public Setting<bool> OnZoneChangeMessage = new(false);
}