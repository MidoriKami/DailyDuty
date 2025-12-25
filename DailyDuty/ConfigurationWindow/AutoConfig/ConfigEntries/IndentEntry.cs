using System;
using KamiToolKit;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class IndentEntry : IConfigEntry {
    public NodeBase BuildNode()
        => throw new InvalidOperationException();

    public void Dispose() { }
}
