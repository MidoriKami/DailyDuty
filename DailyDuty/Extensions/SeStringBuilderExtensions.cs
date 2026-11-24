using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Text;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Extensions;

public static class SeStringBuilderExtensions {
    extension(SeStringBuilder builder) {
        public SeStringBuilder AppendTextColored(ReadOnlySeString text, uint colorCode)
            => builder
                .PushColorType(colorCode)
                .Append(text)
                .PopColorType();

        public SeStringBuilder AppendText(ReadOnlySeString text, Vector4 color)
            => builder
                .PushColorRgba(color)
                .Append(text)
                .PopColor();

        public SeStringBuilder AppendLink(ReadOnlySeString text, DalamudLinkPayload payload, uint color = 576)
            => builder
                .PushColorType(color)
                .PushDalamudLink(payload)
                .Append(text)
                .PopColorType()
                .PopLink();

        public SeStringBuilder AppendDailyDutyPrefix(ReadOnlySeString moduleName)
            => builder
                .AppendTextColored($"[{DailyDutyTag}] ", 45)
                .AppendTextColored($"[{moduleName}] ", 62);
    }

    private static string DailyDutyTag => DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty";
}

public static class RentedSeStringBuilderExtensions {
    extension(RentedSeStringBuilder builder) {
        public SeStringBuilder AppendTextColored(ReadOnlySeString text, uint colorCode)
            => builder.Builder.AppendTextColored(text, colorCode);

        public SeStringBuilder AppendText(ReadOnlySeString text, Vector4 color)
            => builder.Builder.AppendText(text, color);

        public SeStringBuilder AppendLink(ReadOnlySeString text, DalamudLinkPayload payload, uint color = 576)
            => builder.Builder.AppendLink(text, payload, color);

        public SeStringBuilder AppendDailyDutyPrefix(ReadOnlySeString moduleName)
            => builder.Builder.AppendDailyDutyPrefix(moduleName);

        public SeStringBuilder AppendJoined<T>(IEnumerable<uint> ids) where T : struct, IExcelRow<T> {
            var isFirst = true;
            foreach (var id in ids) {
                if (!IDataManager.Get().GetExcelSheet<T>().TryGetRow(id, out var row)) continue;

                if (!isFirst) {
                    builder.AppendNewLine();
                }

                builder.Append(row.DisplayString);
                isFirst = false;
            }

            return builder.Builder;
        }
    }
}

