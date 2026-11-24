---
sessionId: session-261117-183818-2six
---

# Summary

## Requirements

### Overview & Goals
Rework the RaidsNormal and RaidsAlliance modules to eliminate the friction of cross-referencing two dictionaries (`Data.TaskStatus` and `Config.TrackedTasks`) for every operation. Consolidate all raid state into a single `List<RaidEntry>` stored in Config, where only tracked raids have entries.

### Current Problems
- `Data.TaskStatus` (Dictionary<uint, bool>) stores completion for ALL valid raids, not just tracked ones
- `Config.TrackedTasks` (Dictionary<uint, bool>) stores which raids the user wants to track
- `GetIncompleteCount()`, `GetIncompleteTasks()`, and `OnItemEvent` must cross-reference both dictionaries
- `UpdateTrackedTasks()` synchronizes both dictionaries with the valid raid list, duplicating logic
- Config UI uses simple checkboxes but data UI uses `FittedContent`/`VerticalList` — inconsistent with other modules

### Goals
- Single source of truth: `Config.TrackedRaids` (List<RaidEntry>) contains only tracked raids with their completion status
- Config UI shows all valid raids with checkboxes (using `LuminaConfigNode` pattern from ChallengeLog/DutyRoulette)
- Data UI shows only tracked raids with completion status (using `List` with custom `RaidStatusNode`)
- Data classes become minimal (only inheriting `NextReset` from `ModuleDataFile`)
- Reorganize the Nodes folder by purpose for better discoverability
- Migrate existing user data from the old structure to the new structure

## Technical Design

### Current Implementation
- **RaidsNormal.cs** and **RaidsAlliance.cs**: Nearly identical modules. Each tracks raids via `Data.TaskStatus` (completion) and `Config.TrackedTasks` (enabled flag). The data UI uses `FittedContent` with two `VerticalList` pages. The config UI uses `builder.Checkbox()` in a loop.
- **ChallengeLog.cs**, **DutyRoulette.cs**, **HuntMarksDaily.cs**: Use `HashSet<uint>` in Config for tracked entries and `Excel.IsComplete<T>()` for runtime completion checks. Config UI uses `LuminaConfigNode<T>` in a `List` builder.
- **LuminaStatusNode<T>** and **LuminaConfigNode<T>**: Generic list item nodes for Lumina Excel rows. Raids use `ContentFinderCondition`, which isn't currently supported by the `Excel` extension methods.
- **FileBase<T>.TryMigrateStep()**: Existing migration infrastructure that iterates version steps. Config and Data classes can override to migrate from old schemas.

### Key Decisions
- **All raid state in Config**: `TrackedRaids` (List<RaidEntry>) replaces both `TaskStatus` and `TrackedTasks`. Completion is part of the entry, not a separate store.
- **RaidEntry record**: `public record RaidEntry(uint Id, bool Complete);` — simple, serializable, immutable-ish.
- **Config UI uses LuminaConfigNode pattern**: All valid raids displayed with checkboxes. Checking adds a RaidEntry; unchecking removes it. Matches ChallengeLog/DutyRoulette.
- **Data UI uses List with RaidStatusNode**: Only tracked raids displayed with name and completion status. New `RaidStatusNode` class.
- **Nodes reorganized by purpose**: `Display/` for status nodes, `Config/` for config nodes, `Module/` for module UI nodes.
- **Migration in Config only**: Config's `TryMigrateStep` extracts `TrackedTasks` from the `JObject` parameter. Only tracking preferences are migrated; all raids start as incomplete after migration. No direct file reading needed.

### Proposed Changes

#### 1. Reorganize Nodes folder
- Move `LuminaStatusNode.cs` → `Nodes/Display/LuminaStatusNode.cs`
- Move `LuminaConfigNode.cs` → `Nodes/Config/LuminaConfigNode.cs`
- Move `ModuleInfoNode.cs`, `ModuleListNode.cs`, `ConfigInfoNode.cs`, `FeatureInfoNode.cs`, `FeatureListNode.cs` → `Nodes/Module/`
- Update all `using DailyDuty.Nodes;` references (they continue to work since the namespace doesn't change)

#### 2. Create RaidEntry record
- New `RaidEntry.cs` in `Classes/` directory: `public record RaidEntry(uint Id, bool Complete);`

#### 3. Extend Excel.cs for ContentFinderCondition
- Add `ContentFinderCondition` to the `DisplayString` extension method
- Add `ContentFinderCondition` to the `CompletionStatus` extension (returns `Unknown` since there's no game API)

#### 4. Create RaidStatusNode
- New `Nodes/Display/RaidStatusNode.cs`: `ListItemNode<RaidEntry>` that looks up the raid name from `ContentFinderCondition` sheet and displays completion status
- Similar structure to `LuminaStatusNode` but works with `RaidEntry` records

#### 5. Refactor RaidsNormal and RaidsAlliance Config classes
- Replace `Dictionary<uint, bool> TrackedTasks` with `List<RaidEntry> TrackedRaids`
- Override `Version` to return 2
- Override `TryMigrateStep` with a `switch` statement (matching `SystemConfig` pattern). Extract old `TrackedTasks` from `JObject`, create `RaidEntry` records with `Complete = false` for tracked raids. No direct file reading — use `JObject` only.
- Remove `MessageChatChannel` etc. are inherited from `ModuleConfigFile` — no change needed there

#### 6. Refactor RaidsNormal and RaidsAlliance Data classes
- Remove `TaskStatus` property (no longer needed)
- Classes remain but are empty (inherit `NextReset` from `ModuleDataFile`)

#### 7. Refactor RaidsNormal and RaidsAlliance modules
- **BuildDataUi**: Use `builder.List(Config.TrackedRaids, out dataListNode)` with `RaidStatusNode`. Add warning page when no raids are tracked using `FittedContent`.
- **UpdateDataUi**: Simple `dataListNode?.Update()` call
- **BuildConfigUi**: Use `List<ContentFinderCondition, LuminaConfigNode<ContentFinderCondition>>` with all valid raids. Callback adds/removes from `Config.TrackedRaids`.
- **GetIncompleteCount**: `Config.TrackedRaids.Count(e => !e.Complete)`
- **GetIncompleteTasks**: LINQ over `Config.TrackedRaids.Where(e => !e.Complete)`
- **OnItemEvent**: Check `Config.TrackedRaids.Any(e => e.Id == currentDuty)` and update the entry's `Complete` flag
- **ResetAsync**: Set all entries' `Complete` to false
- **GetCompletionStatus**: `Config.TrackedRaids.All(e => e.Complete) ? Complete : Incomplete`
- Remove `UpdateTrackedTasks()` — no longer needed
- Remove `validNormalRaids`/`validAllianceRaids` caching if no longer needed (may still be useful for the config UI)

### Data Models / Contracts

```csharp
// New record in Classes/RaidEntry.cs
public record RaidEntry(uint Id, bool Complete);

// Updated Config classes
public class RaidsNormalConfig : ModuleConfigFile<RaidsNormalConfig> {
    protected override string FileName => "RaidsNormal.config.json";
    public override int Version => 2;
    public List<RaidEntry> TrackedRaids { get; set; } = [];
    
    protected override bool TryMigrateStep(ref int fileVersion, JObject jObject) {
        switch (fileVersion) {
            case 1:
                // Extract old TrackedTasks from JObject
                var trackedTasks = jObject["TrackedTasks"]?.ToObject<Dictionary<uint, bool>>() ?? [];
                // Build new TrackedRaids with all Complete = false
                var trackedRaids = trackedTasks
                    .Where(kv => kv.Value)
                    .Select(kv => new RaidEntry(kv.Key, false))
                    .ToList();
                jObject["TrackedRaids"] = JObject.FromObject(trackedRaids);
                jObject.Remove("TrackedTasks");
                fileVersion = 2;
                return true;
        }
        return base.TryMigrateStep(ref fileVersion, jObject);
    }
}

// Updated Data classes (minimal)
public class RaidsNormalData : ModuleDataFile<RaidsNormalData> {
    protected override string FileName => "RaidsNormal.data.json";
    // No TaskStatus anymore
}
```

### File Structure

```
DailyDuty/
├── Classes/
│   └── RaidEntry.cs                    [NEW]
├── Nodes/
│   ├── Config/
│   │   └── LuminaConfigNode.cs         [MOVED]
│   ├── Display/
│   │   ├── LuminaStatusNode.cs         [MOVED]
│   │   └── RaidStatusNode.cs           [NEW]
│   └── Module/
│       ├── ConfigInfoNode.cs           [MOVED]
│       ├── FeatureInfoNode.cs          [MOVED]
│       ├── FeatureListNode.cs          [MOVED]
│       ├── ModuleInfoNode.cs           [MOVED]
│       └── ModuleListNode.cs           [MOVED]
├── Modules/
│   ├── RaidsNormal.cs                  [MODIFIED]
│   └── RaidsAlliance.cs                [MODIFIED]
└── Utilities/
    └── Excel.cs                        [MODIFIED]
```

### Architecture Diagram

``` mermaid
graph LR
    subgraph Config
        TrackedRaids[List<RaidEntry>]
    end
    
    subgraph DataUI
        RaidStatusNode[RaidStatusNode]
        DataList[List<RaidEntry>]
    end
    
    subgraph ConfigUI
        LuminaConfigNode[LuminaConfigNode<CFC>]
        AllRaids[All Valid Raids]
    end
    
    subgraph Module
        RaidsNormal[RaidsNormal]
        RaidsAlliance[RaidsAlliance]
    end
    
    TrackedRaids --> DataList
    TrackedRaids --> AllRaids
    DataList --> RaidStatusNode
    AllRaids --> LuminaConfigNode
    LuminaConfigNode -->|add/remove| TrackedRaids
    RaidsNormal --> TrackedRaids
    RaidsAlliance --> TrackedRaids
```

### Risks
- **Migration loses completion status**: After migration, all raids start as incomplete. Users will need to re-mark completed raids. Tracking preferences are preserved.
- **RaidEntry immutability**: Because `RaidEntry` is a record (immutable), updating completion requires replacing the entry in the list rather than mutating it in place. Use `TrackedRaids[index] = entry with { Complete = true }` syntax.

# Delivery Steps

### ✓ Step 1: Reorganize Nodes folder and create RaidEntry record
Move existing nodes into subfolders by purpose and create the new `RaidEntry` record.

- Create `Nodes/Display/`, `Nodes/Config/`, and `Nodes/Module/` directories
- Move `LuminaStatusNode.cs` to `Nodes/Display/`
- Move `LuminaConfigNode.cs` to `Nodes/Config/`
- Move `ModuleInfoNode.cs`, `ModuleListNode.cs`, `ConfigInfoNode.cs`, `FeatureInfoNode.cs`, `FeatureListNode.cs` to `Nodes/Module/`
- Create `Classes/RaidEntry.cs` with `public record RaidEntry(uint Id, bool Complete);`
- Verify all `using DailyDuty.Nodes;` references still work (namespace unchanged)

### ✓ Step 2: Extend Excel.cs and create RaidStatusNode
Add `ContentFinderCondition` support to Excel extensions and create the raid status display node.

- Add `ContentFinderCondition` case to `Excel.DisplayString` extension returning `contentFinderCondition.Name`
- Add `ContentFinderCondition` case to `Excel.CompletionStatus` extension returning `CompletionStatus.Unknown`
- Create `Nodes/Display/RaidStatusNode.cs`: `ListItemNode<RaidEntry>` that looks up raid name from `ContentFinderCondition` sheet, displays name and completion status
- RaidStatusNode uses `RatioSplitNode` with 70/30 split (label/status), matching `LuminaStatusNode` pattern

### ✓ Step 3: Refactor Config and Data classes with migration
Update RaidsNormal and RaidsAlliance config and data classes to use the new `RaidEntry` model.

- In `RaidsNormalConfig`: Replace `Dictionary<uint, bool> TrackedTasks` with `List<RaidEntry> TrackedRaids`
- In `RaidsNormalConfig`: Override `Version` to return 2, override `TryMigrateStep` with a `switch` statement that extracts `TrackedTasks` from `JObject` and creates `RaidEntry` records with `Complete = false`. Call `base.TryMigrateStep` as fallback.
- In `RaidsNormalData`: Remove `TaskStatus` property, leave class empty (inherits `NextReset`)
- Apply the same changes to `RaidsAllianceConfig` and `RaidsAllianceData`

### ✓ Step 4: Refactor RaidsNormal module
Rewrite the RaidsNormal module to use the new data model and UI patterns.

- **BuildDataUi**: Use `FittedContent` with a warning page (shown when `Config.TrackedRaids` is empty) and a `List<RaidEntry, RaidStatusNode>` data page
- **UpdateDataUi**: Simple `dataListNode?.Update()` call
- **BuildConfigUi**: Use `List<ContentFinderCondition, LuminaConfigNode<ContentFinderCondition>>` with all valid normal raids. Callback adds/removes from `Config.TrackedRaids`
- **GetIncompleteCount**: `Config.TrackedRaids.Count(e => !e.Complete)`
- **GetIncompleteTasks**: LINQ over `Config.TrackedRaids.Where(e => !e.Complete)` selecting raid names
- **OnItemEvent**: Find the entry in `Config.TrackedRaids` and update its `Complete` flag using record `with` expression
- **ResetAsync**: Set all entries' `Complete` to false using `with` expression
- **GetCompletionStatus**: `Config.TrackedRaids.All(e => e.Complete) ? Complete : Incomplete`
- Remove `UpdateTrackedTasks()`, `statusNodes`, `warningPage`, `dataPage` fields
- Keep `GetValidNormalRaids()` for the config UI

### ✓ Step 5: Refactor RaidsAlliance module
Apply the same refactoring to RaidsAlliance, mirroring the RaidsNormal changes.

- Same `BuildDataUi`, `UpdateDataUi`, `BuildConfigUi`, and helper method changes as RaidsNormal
- Use `GetValidAllianceRaids()` for the config UI (filters for raids containing "Alliance")
- Remove `UpdateTrackedTasks()`, `statusNodes`, `warningPage`, `dataPage` fields

### ✓ Step 6: Build and verify
Verify the project builds and all references are correct.

- Build the solution and verify 0 errors and 0 warnings
- Verify no remaining references to `TrackedTasks` or `TaskStatus` in raids modules
- Verify all node imports resolve correctly after folder reorganization