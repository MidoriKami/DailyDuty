using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces;

internal interface IResettable
{
    public DateTime NextReset { get; set; }

    public bool NeedsResetting()
    {
        return DateTime.UtcNow > NextReset;
    }

    protected DateTime GetNextReset();

    public void DoReset(CharacterSettings settings)
    {
        ResetThis(settings);

        NextReset = GetNextReset();

        Service.Configuration.Save();
    }

    protected void ResetThis(CharacterSettings settings);

}