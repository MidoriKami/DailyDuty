using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Newtonsoft.Json;

namespace DailyDuty.Utilities.Helpers.HuntMarks
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
            if (ImGui.CollapsingHeader("A Realm Reborn"))
            {
                ImGui.Text(huntStruct->RealmReborn.ToJson());
            }

            if (ImGui.CollapsingHeader("Heavensward"))
            {
                ImGui.Text(huntStruct->HeavensWard.ToJson());
            }

            if (ImGui.CollapsingHeader("Stormblood"))
            {
                ImGui.Text(huntStruct->StormBlood.ToJson());
            }            
            
            if (ImGui.CollapsingHeader("Shadowbringers"))
            {
                ImGui.Text(huntStruct->ShadowBringers.ToJson());
            }

            if (ImGui.CollapsingHeader("Endwalker"))
            {
                ImGui.Text(huntStruct->Endwalker.ToJson());
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
