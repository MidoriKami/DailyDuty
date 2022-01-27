using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;

namespace DailyDuty.Localization
{
    public static class Localization
    {
        public static string Get(this Enum value)
        {
            return CheapLoc.Loc.Localize(value.ToString(), value.ToDescription(), Assembly.GetExecutingAssembly());
        }

        public static string ToDescription(this Enum en)
        {
            var type = en.GetType();

            var memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0) return en.ToString();

            var attrs = memInfo[0].GetCustomAttributes(typeof(DisplayText), false);

            return attrs.Length > 0 ? ((DisplayText)attrs[0]).Text : en.ToString();
        }
    }

    public static class Strings
    {
        public enum Common
        {
            [DisplayText("Manually Set Counts")]
            ManuallySetCounts,

            [DisplayText("Persistent Reminders")]
            PersistentReminders,

            [DisplayText("Send a chat notification on non-duty area change.")]
            PersistentReminderDescription
        }

        public enum CustomDelivery
        {
            [DisplayText("Custom Delivery")]
            Category,

            [DisplayText("Remaining Allowances: {0}")]
            RemainingAllowances,
        }
    }
    
    public class DisplayText : Attribute
    {
        public DisplayText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
