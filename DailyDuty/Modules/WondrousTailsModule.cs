using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Structs;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules
{
    internal unsafe class WondrousTailsModule :
        IZoneChangeAlwaysNotification,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IUpdateable
    {
        public CompletionType Type => CompletionType.Weekly;
        public GenericSettings GenericSettings { get; }

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        private static WondrousTailsSettings Settings => Service.CharacterConfiguration.WondrousTails;

        public WondrousTailsModule()
        {
            SignatureHelper.Initialise(this);
        }

        public bool IsCompleted()
        {
            return false;
        }

        public void SendNotification()
        {

        }

        void IZoneChangeAlwaysNotification.SendNotification()
        {

        }

        public void Update()
        {

        }

        public int GetNumStamps()
        {
            return wondrousTails->Stickers;
        }
    }
}
