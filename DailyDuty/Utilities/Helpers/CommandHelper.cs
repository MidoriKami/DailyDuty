using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Utilities.Helpers
{
    internal static class CommandHelper
    {
        public static string? GetSecondaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray.Length == 1)
            {
                return null;
            }

            return stringArray[1];
        }

        public static string? GetPrimaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray[0] == string.Empty)
            {
                return null;
            }

            return stringArray[0];
        }
    }
}
