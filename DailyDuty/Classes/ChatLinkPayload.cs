using System;
using System.Threading;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;

namespace DailyDuty.Classes;

public class ChatLinkPayload : IDisposable {
    private static uint commandIdCounter = 1;

    public DalamudLinkPayload Payload { get; }
    public Action ClickAction { get; }
    private uint CommandId { get; }

    /// <summary>
    /// Creates and registers a chat payload for use with dalamud.
    /// </summary>
    public ChatLinkPayload(Action payloadAction) {
        ClickAction = payloadAction;

        CommandId = commandIdCounter;
        Interlocked.Increment(ref commandIdCounter);

        Payload = IChatGui.Get().AddChatLinkHandler(CommandId, _ => ClickAction());
    }

    public void Dispose()
        => IChatGui.Get().RemoveChatLinkHandler(CommandId);
}
