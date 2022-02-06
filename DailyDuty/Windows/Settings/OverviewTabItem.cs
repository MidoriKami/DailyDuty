using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings
{
    internal class OverviewTabItem : ITabItem
    {
        public string TabName { get; } = "Overview";

        public void Draw()
        {

        }

        public void Dispose()
        {
        }
    }
}
