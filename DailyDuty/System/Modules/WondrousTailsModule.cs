using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImPlotNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Modules
{
    internal unsafe class WondrousTailsModule : Module
    {
        protected readonly Weekly.WondrousTailsSettings Settings = Service.Configuration.WondrousTailsSettings;
        private readonly Stopwatch loginNoticeStopwatch = new();

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        private readonly IntPtr wondrousTailsBasePointer;

        public WondrousTailsModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            var scanner = new SigScanner();
            wondrousTailsBasePointer = scanner.GetStaticAddressFromSig("88 05 ?? ?? ?? ?? 8B 43 18") + 6;
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            if (Settings.Enabled == false) return;

            loginNoticeStopwatch.Start();
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;
            if (loginNoticeStopwatch.IsRunning == true) return;

            if (IsWondrousTailsBookComplete() == true && Settings.NotificationEnabled == true)
            {
                Util.PrintWondrousTails("You have a completed book! Be sure to turn it in!");
            }

            if (ConditionManager.IsBoundByDuty() && Settings.NotificationEnabled == true && !IsWondrousTailsBookComplete())
            {
                lastInstanceWasDuty = true;
                lastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(lastInstanceWasDuty == true)
            {
                OnDutyEndNotification();
                lastInstanceWasDuty = false;
            }
            else
            {
                lastInstanceWasDuty = false;
            }
        }

        private void OnDutyEndNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            if (buttonState == ButtonState.Completable)
            {
                Util.PrintWondrousTails("You can claim a stamp for the last instance!");
            }
        }

        private void OnDutyStartNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (GetSecondChanceCount() > 0)
                    {
                        Util.PrintWondrousTails($"This instance is available for a stamp if you re-roll it! You have {GetSecondChanceCount()} Re-Rolls Available.");
                    }
                    break;

                case ButtonState.AvailableNow:
                    Util.PrintWondrousTails("A stamp is already available for this instance.");
                    break;

                case ButtonState.Completable:
                    Util.PrintWondrousTails("Completing this instance will reward you with a stamp!");
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        public override void Update()
        {
            if (Settings.Enabled == false) return;
            if (loginNoticeStopwatch.IsRunning == false) return;

            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            if (loginNoticeStopwatch.Elapsed >= TimeSpan.FromSeconds(5) && loginNoticeStopwatch.IsRunning)
            {
                if (IsWondrousTailsBookComplete() == true)
                {
                    Util.PrintWondrousTails("You have a completed book! Be sure to turn it in!");
                }

                loginNoticeStopwatch.Stop();
            }
        }

        private IEnumerable<(ButtonState, List<uint>)> GetAllTaskData()
        {
            byte[] taskID = new byte[16];
            var taskAddress = wondrousTailsBasePointer;
            Marshal.Copy(taskAddress, taskID, 0, 16);

            var result = new (ButtonState, List<uint>)[16];

            for (int i = 0; i < 16; ++i)
            {
                var taskButtonState = GetTaskCompletionArray()[i];
                var instances = GetInstanceListFromID(taskID[i]);

                PluginLog.Information($"ButtonState: {taskButtonState}, Instances:{"{ " + string.Join(", ", instances) + " }"}");

                result[i] = (taskButtonState, instances);

            }

            return result;
        }

        private ButtonState[] GetTaskCompletionArray()
        {
            byte[] completionStatus = new byte[4];
            var completionAddressStart = wondrousTailsBasePointer + 28;
            Marshal.Copy(completionAddressStart, completionStatus, 0, 4);

            ButtonState[] taskCompletion = new ButtonState[16];

            for (int y = 0; y < 4; ++y)
            {
                var byteValue = completionStatus[y];

                var fourth = (byteValue & 0b11000000) >> 6;
                var third = (byteValue & 0b00110000) >> 4;
                var second = (byteValue & 0b00001100) >> 2;
                var first = (byteValue & 0b00000011);

                taskCompletion[y*4 + 0] = ConvertToButtonState(first);
                taskCompletion[y*4 + 1] = ConvertToButtonState(second);
                taskCompletion[y*4 + 2] = ConvertToButtonState(third);
                taskCompletion[y*4 + 3] = ConvertToButtonState(fourth);
            }

            return taskCompletion;
        }

        private ButtonState ConvertToButtonState(int value)
        {
            if (value == 2)
                return ButtonState.Unavailable;

            if (value == 1)
                return ButtonState.AvailableNow;

            if (value == 0)
                return ButtonState.Completable;

            return ButtonState.Unknown;
        }
        
        private static int CountSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }

            return count;
        }

        public bool IsWondrousTailsBookComplete()
        {
            var address = wondrousTailsBasePointer + 20;
            byte[] data = new byte[2];
            Marshal.Copy(address, data, 0, 2);

            var playerHasBook = PlayerHasBook();
            var fullNumber = (data[0] << 8) | data[1];

            var numBits = CountSetBits(fullNumber);

            return numBits == 9 && playerHasBook;
        }
        
        private int GetSecondChanceCount()
        {
            var address = wondrousTailsBasePointer + 26;
            byte[] secondChance = new byte[2];
            Marshal.Copy(address, secondChance, 0, 2);

            // value lies between bytes, little endian xxxxxxx0 1yyyyyyy

            // correct endianess
            var value = (secondChance[1] << 8) + secondChance[0];

            // mask and shift out value
            var result = (value & 0x0180) >> 7;
            
            return result;
        }

        private (ButtonState, List<uint>)? FindNode(uint instanceID)
        {
            foreach (var (buttonState, instanceList) in GetAllTaskData())
            {
                if (instanceList.Contains(instanceID))
                {
                    return (buttonState, instanceList);
                }
            }

            return null;
        }

        private List<uint> GetInstanceListFromID(uint id)
        {
            var values = TryGetFromDatabase(id);

            if (values != null)
            {
                return new() {values.Value};
            }

            switch (id)
            {
                // Dungeons Lv 1-49
                case 1:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 50
                case 2:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 50)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 51-59
                case 3:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 51 and <= 59)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 60
                case 4:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 60)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 61-69
                case 59:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 61 and <= 69)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 70
                case 60:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 70)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 71-79
                case 85:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 80
                case 86:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons lv 81-89
                case 108:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 81 and <= 89)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 90
                case 109:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Palace of the Dead / Heaven on High
                case 53:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 46:
                    // todo: Find Treasure Map Instance List
                    return new List<uint>();

                // Rival Wings
                case 67:
                    return new List<uint>();

            }

            PluginLog.Information($"[WondrousTails] Unrecognized ID: {id}");
            return new List<uint>();
        }

        private uint? TryGetFromDatabase(uint id)
        {
            var instanceContentData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>()
                !.GetRow(id)
                !.Data;

            if (instanceContentData < 20000)
            {
                return null;
            }

            var data = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(c => c.Content == instanceContentData)
                .Select(c => c.TerritoryType.Value!.RowId)
                .FirstOrDefault();

            return data;
        }

        private bool PlayerHasBook()
        {
            var address = wondrousTailsBasePointer + 24;
            byte[] data = new byte[1];
            Marshal.Copy(address, data, 0, 1);

            var maskedData = (data[0] & 0b00010000) >> 4;

            return maskedData > 0;
        }

        private AddonWeeklyBingo* GetWondrousTailsPointer()
        {
            return (AddonWeeklyBingo*)Service.GameGui.GetAddonByName("WeeklyBingo", 1);
        }

        public override void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        public override bool ModuleIsCompleted()
        {
            return IsWondrousTailsBookComplete();
        }
    }
}
