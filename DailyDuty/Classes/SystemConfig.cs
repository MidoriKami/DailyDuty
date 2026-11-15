using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Classes;

public class SystemConfig : FileBase<SystemConfig> {
    protected override string FileName => "system.config.json";
    public override int Version => 3;

    public bool EnableSceneEventLogging;
    public bool IsDebugMode = false;

    public HashSet<string> EnabledModules = [];

    protected override bool TryMigrateStep(ref int fileVersion, JObject jObject) {
        switch (fileVersion) {
            case < 3:
                // Older configs may have version 0, 1, or 2 due to a bug.
                // These configs remain structurally valid; bump directly to v3.
                fileVersion = 3;
                return true;
        }

        return base.TryMigrateStep(ref fileVersion, jObject);
    }
}
