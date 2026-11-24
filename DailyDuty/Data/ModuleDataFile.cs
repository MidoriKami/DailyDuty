using System;

namespace DailyDuty.Data;

public abstract class ModuleDataFile<T> : FileBase<T> where T : ModuleDataFile<T>, new() {

    /// <summary>
    /// Gets or sets when this module expects to be reset next.
    /// </summary>
    public DateTime NextReset { get; set; }
}
