using DailyDuty.Configuration.Components;
using System;
using System.Collections.Generic;
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
        Service.PluginInterface.RemoveChatLinkHandler((uint)type + 1000);
        var payload = Service.PluginInterface.AddChatLinkHandler((uint)type + 1000, payloadAction);

        ChatLinkPayloads.Add(new ChatLinkPayload((uint)type + 1000, type, payload));

        return payload;
    }
}