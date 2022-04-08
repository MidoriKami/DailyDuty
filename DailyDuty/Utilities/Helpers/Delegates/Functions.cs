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

        internal static class Other
        {
            internal static class MobHunt
            {
                public delegate void MarkObtained(void* a1, byte a2, int a3);
                public delegate void MobKill(void* a1, byte a2, uint a3, uint a4);
                public delegate void MarkComplete(void* a1, byte a2);
            }
        }
    }
}
