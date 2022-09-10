using System;

namespace DailyDuty.Interfaces;

internal interface IInfoBoxTableRow
{
    Tuple<Action?, Action?> GetConfigurationRow();
    Tuple<Action?, Action?> GetDataRow();
}