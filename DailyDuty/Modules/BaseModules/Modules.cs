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

public class ModuleTaskData<T> : ModuleData where T : struct, IExcelRow<T> {
    public LuminaTaskDataList<T> TaskData = [];

    protected override void DrawModuleData()
        => TaskData.Draw();
}

public class ModuleTaskConfig<T> : ModuleConfig where T : struct, IExcelRow<T> {
    public LuminaTaskConfigList<T> TaskConfig = [];

    protected override void DrawModuleConfig() {
        ConfigChanged |= TaskConfig.Draw();
    }
}

public static class Modules {
    public abstract class Daily<T, TU> : Module<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Daily;

        public override DateTime GetNextReset() 
            => Time.NextDailyReset();
    }

    public abstract class Weekly<T, TU> : Module<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Weekly;
        
        public override DateTime GetNextReset()
            => Time.NextWeeklyReset();
    }

    public abstract class Special<T, TU> : Module<T, TU> where T : ModuleData, new() where TU : ModuleConfig, new() {
        public override ModuleType ModuleType => ModuleType.Special;
    }

    public abstract class DailyTask<T, TU, TV> : Daily<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : struct, IExcelRow<TV> {
        public override bool HasTooltip => true;
        
        public override string TooltipText { get; protected set; } = string.Empty;

        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
            System.TodoListController.Refresh();
        }
    }

    public abstract class WeeklyTask<T, TU, TV> : Weekly<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : struct, IExcelRow<TV> {
        public override bool HasTooltip => true;
        
        public override string TooltipText { get; protected set; } = string.Empty;

        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
            System.TodoListController.Refresh();
        }
    }

    public abstract class SpecialTask<T, TU, TV> : Special<T, TU> where T : ModuleTaskData<TV>, new() where TU : ModuleTaskConfig<TV>, new() where TV : struct, IExcelRow<TV> {
        public override bool HasTooltip => true;

        public override string TooltipText { get; protected set; } = string.Empty;
        
        protected int IncompleteTaskCount;

        protected override void UpdateTaskData() {
            IncompleteTaskCount = GetIncompleteCount(Config.TaskConfig, Data.TaskData);
            TooltipText = string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));
            System.TodoListController.Refresh();
        }
    }
    
    private static int GetIncompleteCount<TV>(LuminaTaskConfigList<TV> config, LuminaTaskDataList<TV> data) where TV : struct, IExcelRow<TV> {
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

    private static IEnumerable<string> GetIncompleteRows<TV>(LuminaTaskConfigList<TV> config, LuminaTaskDataList<TV> data) where TV : struct, IExcelRow<TV> {
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