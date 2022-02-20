using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;

namespace DailyDuty.Interfaces
{
    internal interface IOverlay : IDisposable
    {
        public OverlayName OverlayName { get; }
    }
}
