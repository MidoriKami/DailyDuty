using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.Interfaces
{
    internal interface IChatHandler
    {
        public void HandleChat(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled);
    }
}
