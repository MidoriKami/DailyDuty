using System.Collections.Generic;
using System.Linq;
using DailyDuty.Enums;

namespace DailyDuty.Classes;

public class ModuleInfo {
	public int Version => ChangeLog.Max(changelog => changelog.Version);
	public required string DisplayName { get; init; }
    public required string FileName { get; init; }
	public required ModuleType Type { get; init; }
	public required List<ChangeLogInfo> ChangeLog { get; init; } = [];
	public List<string> Tags { get; init; } = [];
}
