using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings.SettingsHeaders;

internal class TodoWindowConfiguration : ICollapsibleHeader
{
    public void Dispose()
    {
            
    }

    public string HeaderText { get; } = "Todo Window Configuration";
    void ICollapsibleHeader.DrawContents()
    {

    }
}