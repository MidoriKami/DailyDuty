using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Utility;
using Lumina.Text;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Extensions;

public static class StringBuilderExtensions {
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
    }
}

public static class RentedSeStringBuilderExtensions {
    extension(RentedSeStringBuilder builder) {
        public SeStringBuilder AppendTextColored(ReadOnlySeString text, uint colorCode)
            => builder.Builder.AppendTextColored(text, colorCode);

        public SeStringBuilder AppendText(ReadOnlySeString text, Vector4 color)
            => builder.Builder.AppendText(text, color);

        public SeStringBuilder AppendLink(ReadOnlySeString text, DalamudLinkPayload payload, uint color = 576)
            => builder.Builder.AppendLink(text, payload, color);
    }
}
