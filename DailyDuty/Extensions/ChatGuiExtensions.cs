using System;
using DailyDuty.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;

namespace DailyDuty.Extensions;

public static class ChatGuiExtensions {
	extension(IChatGui chatGui) {
		public void PrintMessage(XivChatType channel, string moduleName, string message) => chatGui.Print(new XivChatEntry {
			Type = channel,
			Message = new SeStringBuilder()
	          .AddUiForeground($"[{DailyDutyTag}] ", 45)
	          .AddUiForeground($"[{moduleName}] ", 62)
	          .AddText(message)
	          .Build(),
		});

		/// <summary> Prints to Debug Channel </summary>
		public void PrintTaggedMessage(string message, string tag) => chatGui.Print(new XivChatEntry {
			Type = XivChatType.Debug,
			Message = new SeStringBuilder()
	          .AddUiForeground($"[{DailyDutyTag}] ", 45)
	          .AddUiForeground($"[{tag}] ", 62)
	          .AddText(message)
	          .Build(),
		});

		public void PrintPayloadMessage(XivChatType channel, PayloadId payload, string moduleName, string message) => chatGui.Print(new XivChatEntry {
			Type = channel,
			Message = BuildPayloadString(payload, moduleName, message),
		});
	}
	
	private static string DailyDutyTag => DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty";

	private static SeString BuildPayloadString(PayloadId payloadId, string moduleName, string message) {
		if ((System.SystemConfig?.EnableChatLinks ?? false) && payloadId is not PayloadId.Unset) {
			return new SeStringBuilder()
		       .AddUiForeground($"[{DailyDutyTag}] ", 45)
		       .AddUiForeground($"[{moduleName}] ", 62)
		       .Add(System.PayloadController.GetPayload(payloadId))
		       .AddUiForeground(message, 576)
		       .Add(RawPayload.LinkTerminator)
		       .Build();
		}
		
		return new SeStringBuilder()
	       .AddUiForeground($"[{DailyDutyTag}] ", 45)
	       .AddUiForeground($"[{moduleName}] ", 62)
	       .AddUiForeground(message, 576)
	       .Build();
	}
}
