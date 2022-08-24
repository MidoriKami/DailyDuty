using DailyDuty.Configuration.Enums;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Configuration.Components;

internal record TeleportInfo(uint CommandID, TeleportLocation Target, Aetheryte Aetherite);
