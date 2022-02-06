using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings
{
    internal class ConfigurationTabItem : ITabItem
    {
        public static bool EditModeEnabled = false;

        public void Dispose()
        {
            
        }

        public string TabName => "Configuration";

        public void Draw()
        {

        }
    }
}
