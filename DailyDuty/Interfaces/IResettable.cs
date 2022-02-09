using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Utilities;

namespace DailyDuty.Interfaces;

internal interface IResettable
{
    public DateTime NextReset { get; set; }

    public bool NeedsResetting()
    {
        return Time.Now() > NextReset;
    }

    protected DateTime GetNextReset();

    public void DoReset(CharacterSettings settings)
    {
        ResetThis(settings);

        NextReset = GetNextReset();
    }

    protected void ResetThis(CharacterSettings settings);

}