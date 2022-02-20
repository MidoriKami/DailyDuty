using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;

namespace DailyDuty.Interfaces
{
    public interface IWindow : IDisposable
    {
        public WindowName WindowName { get; }

    }
}
