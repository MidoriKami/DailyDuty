using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Interfaces;

internal interface ITabItem : IDisposable
{
    string TabName { get; }

    void Draw();

}