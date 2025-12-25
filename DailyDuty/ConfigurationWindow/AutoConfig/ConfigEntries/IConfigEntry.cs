using System;
using KamiToolKit;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public interface IConfigEntry : IDisposable {
    NodeBase BuildNode();
}
