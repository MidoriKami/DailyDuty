using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Interfaces;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Newtonsoft.Json;

namespace DailyDuty.Windows.HuntMark
{
    internal unsafe class HuntMarkHelper : Window, IWindow
    {
        public new WindowName WindowName => WindowName.Debug_HuntMarkHelper;

        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private readonly MobHuntStruct* huntStruct = null;

        public HuntMarkHelper() : base("DailyDuty HuntStructDebugger")
        {
            SignatureHelper.Initialise(this);

            Service.WindowSystem.AddWindow(this);

            IsOpen = true;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            IsOpen = true;
        }

        public override void Draw()
        {
            if (IsOpen == false) return;

            PrintStruct();
        }

        private void PrintStruct()
        {
            for (var i = HuntMarkType.RealmReborn_LevelOne; i <= HuntMarkType.Endwalker_Elite; ++i)
            {
                if (ImGui.CollapsingHeader(i.ToString()))
                {
                    var data = huntStruct->Get(i);

                    ImGui.Text(data.ToJson());
                }
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
