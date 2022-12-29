using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.DataModels;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.System;

internal class ChatPayloadManager : IDisposable
{
    private List<ChatLinkPayload> ChatLinkPayloads { get; } = new();

    public void Dispose()
    {
        foreach (var payload in ChatLinkPayloads)
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint)payload.Type + 1000);
        }
    }

    public DalamudLinkPayload AddChatLink(ChatPayloads type, Action<uint, SeString> payloadAction)
    {
        // If the payload is already registered
        var payload = ChatLinkPayloads.FirstOrDefault(linkPayload => linkPayload.CommandID == (uint) type + 1000)?.Payload;
        if (payload != null) return payload;

        // else
        Service.PluginInterface.RemoveChatLinkHandler((uint)type + 1000);
        payload = Service.PluginInterface.AddChatLinkHandler((uint)type + 1000, payloadAction);

        ChatLinkPayloads.Add(new ChatLinkPayload((uint)type + 1000, type, payload));

        return payload;
    }
}