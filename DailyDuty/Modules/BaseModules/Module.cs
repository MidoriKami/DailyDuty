using System;
using System.Collections.Generic;
using System.ComponentModel;
using DailyDuty.Classes;
using DailyDuty.Models;
using Lumina.Excel;

namespace DailyDuty.Modules.BaseModules;

public enum ModuleType {
    [Description("Daily")]
    Daily,
    
    [Description("Weekly")]
    Weekly,
    
    [Description("Special")]
    Special,
}

public class ModuleTaskData<T> : ModuleData where T : ExcelRow {
    public LuminaTaskDataList<T> TaskData = [];

    protected override void DrawModuleData()
        => TaskData.Draw();
}

public class ModuleTaskConfig<T> : ModuleConfig where T : ExcelRow {
    public LuminaTaskConfigList<T> TaskConfig = [];

    protected override bool DrawModuleConfig()
        => TaskConfig.Draw();
}

public static class Module {
    public abstract class DailyModule<T, TU> : BaseModule<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Daily;

        public override DateTime GetNextReset() 
            => Time.NextDailyReset();
    }

    public abstract class WeeklyModule<T, TU> : BaseModule<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Weekly;
        
        public override DateTime GetNextReset()
            => Time.NextWeeklyReset();
    }

    public abstract class SpecialModule<T, TU> : BaseModule<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Special;
    }

    public abstract class DailyTaskModule<T, TU, TV> : DailyModule<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : ExcelRow {
        public override bool HasTooltip => true;
        
        public override string TooltipText { get; protected set; } = string.Empty;

        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
        }
    }

    public abstract class WeeklyTaskModule<T, TU, TV> : WeeklyModule<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : ExcelRow {
        public override bool HasTooltip => true;
        
        public override string TooltipText { get; protected set; } = string.Empty;

        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
        }
    }

    public abstract class SpecialTaskModule<T, TU, TV> : SpecialModule<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : ExcelRow {
        public override bool HasTooltip => true;

        public override string TooltipText { get; protected set; } = string.Empty;
        
        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
        }
    }
    
    private static int GetIncompleteCount<TV>(LuminaTaskConfigList<TV> config, LuminaTaskDataList<TV> data) where TV : ExcelRow {
        if (config.Count != data.Count) throw new Exception("Task and Data array size are mismatched. Unable to calculate IncompleteCount.");

        var count = 0;
        for (var i = 0; i < config.Count; i++) {
            var configTask = config.ConfigList[i];
            var dataTask = data.DataList[i];

            if (configTask.RowId != dataTask.RowId) throw new Exception($"Task and Data rows are mismatched. Unable to calculate IncompleteCount.\nConfig RowId: {configTask.RowId} Data RowId: {dataTask.RowId}.");

            if (configTask.Enabled) {
                var isCountableTaskIncomplete = configTask.TargetCount != 0 && dataTask.CurrentCount < configTask.TargetCount;
                var isNonCountableTaskIncomplete = configTask.TargetCount == 0 && !dataTask.Complete;
                
                if (isCountableTaskIncomplete || isNonCountableTaskIncomplete) count++;
            }
        }

        return count;
    }

    private static IEnumerable<string> GetIncompleteRows<TV>(LuminaTaskConfigList<TV> config, LuminaTaskDataList<TV> data) where TV : ExcelRow {
        if (config.Count != data.Count) throw new Exception("Task and Data array size are mismatched. Unable to calculate IncompleteCount.");

        for (var i = 0; i < config.Count; i++) {
            var configTask = config.ConfigList[i];
            var dataTask = data.DataList[i];

            if (configTask.RowId != dataTask.RowId) throw new Exception($"Task and Data rows are mismatched. Unable to calculate IncompleteCount.\nConfig RowId: {configTask.RowId} Data RowId: {dataTask.RowId}.");

            if (configTask.Enabled) {
                var isCountableTaskIncomplete = configTask.TargetCount != 0 && dataTask.CurrentCount < configTask.TargetCount;
                var isNonCountableTaskIncomplete = configTask.TargetCount == 0 && !dataTask.Complete;
                
                if (isCountableTaskIncomplete || isNonCountableTaskIncomplete) yield return configTask.Label();
            }
        }
    }
}