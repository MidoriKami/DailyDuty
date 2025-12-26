using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Enums;
using DailyDuty.Extensions;

namespace DailyDuty.Classes;

public class ModuleInfo {
	public int Version => ChangeLog.Max(changelog => changelog.Version);
	public required string DisplayName { get; init; }
    public required string FileName { get; init; }
	public required ModuleType Type { get; init; }
	public required List<ChangeLogInfo> ChangeLog { get; init; } = [];
	public List<string> Tags { get; init; } = [];
    public PayloadId MessageClickAction { get; init; }
    
    public bool IsMatch(string searchTerm) {
        if (DisplayName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) return true;
        if (Type.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) return true;
        if (Tags.Any(tag => tag.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) return true;
        
        return false;
    }
}
