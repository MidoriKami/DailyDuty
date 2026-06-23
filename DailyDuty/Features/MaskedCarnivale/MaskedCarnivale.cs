using DailyDuty.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.MaskedCarnivale;

public class MaskedCarnivale : Module<MaskedCarnivaleConfig, MaskedCarnivaleData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.Masked_Carnivale,
        FileName = "MaskedCarnivale",
        Type = ModuleType.Weekly,
        Tags = ["Teleport", "Tickets", "Seals"],
    };

    public override DataNodeBase DataNode => new MaskedCarnivaleDataNode(this);
    public override ConfigNodeBase ConfigNode => new MaskedCarnivaleConfigNode(this);

    protected override MaskedCarnivaleConfig MigrateConfig(JObject objectData)
        => MaskedCarnivaleMigration.Migrate(objectData);

    protected override async Task OnModuleEnable() {
        await Services.Framework.Run(() => {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "AOZContentResult", AozContentResultPostSetup);
        });

        // Fix for configs that don't have these data entries, newly generated configs will default with them.
        ModuleData.TaskData.TryAdd(12449, false);
        ModuleData.TaskData.TryAdd(12448, false);
        ModuleData.TaskData.TryAdd(12447, false);
    }

    protected override async Task OnModuleDisable() {
        await Services.Framework.Run(() => {
            Services.AddonLifecycle.UnregisterListener(AozContentResultPostSetup);
        });
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{GetIncompleteCount()} {Strings.Challenge_s__Incomplete}",
        PayloadId = PayloadId.UldahTeleport,
    };

    protected override TodoTooltip GetTooltip() => new() {
        TooltipText = string.Join("\n", GetIncompleteTasks()),
        ClickAction = PayloadId.UldahTeleport,
    };

    public override void Reset() {
        foreach (var key in ModuleData.TaskData.Keys) {
            ModuleData.TaskData[key] = false;
        }
    }

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus()
        => GetIncompleteCount() is not 0 ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    private int GetIncompleteCount() {
        var count = 0;

        foreach (var tracked in ModuleConfig.TrackedTasks) {
            if (!ModuleData.TaskData.TryGetValue((int)tracked, out var taskCompleted) || !taskCompleted) {
                count++;
            }
        }

        return count;
    }

    protected override unsafe void OnModuleUpdate() {
        if (AgentAozContentBriefing.Instance() is not null && AgentAozContentBriefing.Instance()->IsAgentActive()) {
            ModuleData.TaskData.TryAdd(12449, false);
            ModuleData.TaskData.TryAdd(12448, false);
            ModuleData.TaskData.TryAdd(12447, false);

            foreach (var (addonId, taskStatus) in ModuleData.TaskData) {
                var status = addonId switch {
                    12449 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Novice),
                    12448 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Moderate),
                    12447 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Advanced),
                    _ => throw new ArgumentOutOfRangeException(),
                };

                if (taskStatus != status) {
                    ModuleData.TaskData[addonId] = status;
                    ModuleData.MarkDirty();
                }
            }
        }
    }

    private unsafe void AozContentResultPostSetup(AddonEvent eventType, AddonArgs addonInfo) {
        var addon = (AtkUnitBase*)addonInfo.Addon.Address;

        if (addon->AtkValues[112] is not { Type: AtkValueType.UInt, UInt: var completionIndex }) throw new Exception("Type Mismatch Exception");
        if (addon->AtkValues[114] is not { Type: AtkValueType.Bool, Byte: var completionStatus }) throw new Exception("Type Mismatch Exception");

        var addonId = completionIndex switch {
            0 => 12449,
            1 => 12448,
            2 => 12447,
            _ => throw new ArgumentOutOfRangeException(),
        };

        ModuleData.TaskData.TryAdd(addonId, completionStatus != 0);
        ModuleData.TaskData[addonId] = completionStatus != 0;
        ModuleData.MarkDirty();
    }

    private IEnumerable<string> GetIncompleteTasks()
        => ModuleConfig.TrackedTasks
            .Where(job => !ModuleData.TaskData[(int)job])
            .Select(job => Services.DataManager.GetExcelSheet<Addon>().GetRow(job).Text.ToString());
}
