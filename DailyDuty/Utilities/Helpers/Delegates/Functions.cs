using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Component.GUI;
// ReSharper disable InconsistentNaming

namespace DailyDuty.Utilities.Helpers.Delegates
{
    internal static unsafe class Functions
    {
        internal static class Addon
        {
            public delegate void Draw(AtkUnitBase* atkUnitBase);
            public delegate void* Finalize(AtkUnitBase* atkUnitBase);
            public delegate byte Update(AtkUnitBase* atkUnitBase);
            public delegate byte OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
        }

        internal static class Agent
        {
            public delegate void* ReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, int eventDataItemCount, int senderID);

            internal static class AgentContentsFinder
            {
                public delegate void* Show(void* a1);
            }

            internal static class LotteryDaily
            {
                public delegate void* Show(AgentInterface* addon, void* a2, void* a3);
            }
        }

    }
}
