using System;
using DailyDuty.Interfaces;
using DailyDuty.Structs;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Newtonsoft.Json;

namespace DailyDuty.Windows.HuntHelperWindow
{
    internal unsafe class HuntHelperWindow : Window, IDisposable, ICommand
    {

        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private readonly MobHuntStruct* huntStruct = null;

        public HuntHelperWindow() : base("DailyDuty HuntStructDebugger")
        {
            SignatureHelper.Initialise(this);

            Service.WindowSystem.AddWindow(this);
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            if (!Service.LoggedIn || Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void Draw()
        {
            PrintStruct();
        }

        private void PrintStruct()
        {
            for (var i = HuntMarkType.RealmRebornLevelOne; i <= HuntMarkType.EndwalkerElite; ++i)
            {
                if (ImGui.CollapsingHeader(i.ToString()))
                {
                    var data = huntStruct->Get(i);

                    ImGui.Text(data.ToJson());
                }
            }
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == "hunthelper")
            {
                IsOpen = true;
            }
        }
    }

    public static class MobHuntExtensions
    {
        public static string ToJson<T>(this T obj)
        { 
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}
