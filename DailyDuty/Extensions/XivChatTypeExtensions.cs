using System;
using System.Linq;
using Dalamud.Game.Text;

namespace DailyDuty.Extensions;

public static class XivChatTypeExtensions {
    extension(XivChatType chatType) {
        public string Description => chatType.GetDetails()?.FancyName ?? chatType.ToString();
    }
    
    public static XivChatType Parse(string chatType) {
        var result = Enum.GetValues<XivChatType>().Where(type => type.GetDetails()?.FancyName == chatType).FirstOrDefault();
        
        return result == default ? XivChatType.Debug : result;
    }
}
