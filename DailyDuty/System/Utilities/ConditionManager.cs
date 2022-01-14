using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Utilities
{

    internal class ConditionManager
    {
        private readonly string ExitNodeText;

        public ConditionManager()
        {
            ExitNodeText = Service.DataManager.GetExcelSheet<EObjName>()!
                .GetRow(2000139)!
                .Singular;
        }

        private bool IsDutyEnded()
        {
            return Service.ObjectTable
                .Any(o => o.ObjectKind == ObjectKind.EventObj && o.Name.ToString().ToLower() == ExitNodeText && Util.IsTargetable(o));
        }

        public static bool IsBoundByDuty()
        {
            var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
            var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
            var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];

            // Triggers when Queue is started
            //var boundBy97 = Service.Condition[ConditionFlag.BoundToDuty97];

            return baseBoundByDuty || boundBy56 || boundBy95;
        }

        public static bool IsInAreaTransition()
        {
            var baseTransition = Service.Condition[ConditionFlag.BetweenAreas];
            var transition51 = Service.Condition[ConditionFlag.BetweenAreas51];
            var beingMoved = Service.Condition[ConditionFlag.BeingMoved];
            var jumping61 = Service.Condition[ConditionFlag.Jumping61];

            return baseTransition || transition51 || beingMoved || jumping61;
        }
    }
}
