using System;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules
{
    internal unsafe class FashionReportModule :
        IResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IDisposable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        public static FashionReportSettings Settings => Service.CharacterConfiguration.FashionReport;
        public string DisplayName => Strings.Module.FashionReportLabel;
        public Action? ExpandedDisplay => null;

        private readonly DalamudLinkPayload goldSaucerTeleport;

        private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6, byte a7);

        [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(GoldSaucerUpdate))]
        private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;

        private void* GoldSaucerUpdate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6, byte a7)
        {
            try
            {
                if (Service.TargetManager.Target?.DataId == 1025176)
                {
                    int allowances = Settings.AllowancesRemaining;
                    int score = Settings.HighestWeeklyScore;

                    switch (a7)
                    {
                        // When speaking to Masked Rose, gets update information
                        case 5:
                            allowances = a6[1];
                            score = a6[0];
                            break;

                        // During turn in, gets new score
                        case 3:
                            score = a6[0];
                            break;

                        // During turn in, gets new allowances
                        case 1:
                            allowances = a6[0];
                            break;
                    }

                    if (Settings.AllowancesRemaining != allowances)
                    {
                        Settings.AllowancesRemaining = allowances;
                        Service.LogManager.LogMessage(ModuleType.FashionReport,
                            "Remaining Allowances Updated " + Settings.AllowancesRemaining);
                        Service.CharacterConfiguration.Save();
                    }

                    if (Settings.HighestWeeklyScore != score)
                    {
                        Settings.HighestWeeklyScore = score;
                        Service.LogManager.LogMessage(ModuleType.FashionReport,
                            "Highest Score Updated " + Settings.HighestWeeklyScore);
                        Service.CharacterConfiguration.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "[Fashion Report] Unable to get data from Gold Saucer Update");
            }

            return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, a6, a7);
        }

        public FashionReportModule()
        {
            SignatureHelper.Initialise(this);

            goldSaucerUpdateHook?.Enable();

            goldSaucerTeleport = Service.TeleportManager.GetPayload(ChatPayloads.GoldSaucerTeleport);
        }

        public void Dispose()
        {
            goldSaucerUpdateHook?.Dispose();
        }
        
        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                switch(Settings.Mode)
                {
                    case FashionReportMode.Single:
                        if(Settings.AllowancesRemaining == 4)
                        {
                            Chat.Print(Strings.Module.FashionReportLabel, Strings.Module.CustomDeliveryAllowancesLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
                        }
                        break;
                    case FashionReportMode.Plus80:
                        if (Settings.HighestWeeklyScore < 80)
                        {
                            Chat.Print(Strings.Module.FashionReportLabel, $"{Settings.AllowancesRemaining} " + Strings.Module.CustomDeliveryAllowancesLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
                            Chat.Print(Strings.Module.FashionReportLabel,$"{Settings.HighestWeeklyScore} " + Strings.Module.FashionReportHighestScoreLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
                        }
                        break;
                    case FashionReportMode.All:
                        Chat.Print(Strings.Module.FashionReportLabel, $"{Settings.AllowancesRemaining} " + Strings.Module.CustomDeliveryAllowancesLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
                        break;
                }
            }
        }

        DateTime IResettable.GetNextReset() => Time.NextWeeklyReset();

        void IResettable.ResetThis()
        {
            Service.LogManager.LogMessage(ModuleType.FashionReport, "Weekly Reset - Resetting");

            Settings.AllowancesRemaining = 4;
            Settings.HighestWeeklyScore = 0;
        }

        public bool IsCompleted() => FashionReportComplete();

        private bool FashionReportComplete()
        {
            if (FashionReportAvailable() == false) return true;

            // Zero is always "Complete"
            // Four is always "Incomplete"
            if (Settings.AllowancesRemaining == 0) return true;
            if (Settings.AllowancesRemaining == 4) return false;

            // If this line is reached, then we have between 1 and 3 remaining allowances (inclusive)
            switch (Settings.Mode)
            {
                case FashionReportMode.Single:
                case FashionReportMode.All when Settings.AllowancesRemaining == 0:
                case FashionReportMode.Plus80 when Settings.HighestWeeklyScore >= 80:
                    return true;

                default:
                    return false;
            }
        }

        public bool FashionReportAvailable()
        {
            var reportOpen = Time.NextFashionReportReset();
            var reportClosed = Time.NextWeeklyReset();

            var now = DateTime.UtcNow;

            return now > reportOpen && now < reportClosed;
        }
    }
}
