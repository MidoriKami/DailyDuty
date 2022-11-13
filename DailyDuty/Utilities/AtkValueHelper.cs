using System;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.Utilities;

public static class AtkValueHelper
{
    public static unsafe void PrintAtkValue(AtkValue value, int index)
    {
        switch (value.Type)
        {
            case ValueType.Int:
                PluginLog.Debug($"[{index:D3}] [{"int", 7}]: {value.Int}");
                break;
            case ValueType.Bool:
                PluginLog.Debug($"[{index:D3}] [{"bool", 7}]: {(value.Byte != 0 ? "true" : "false")}");
                break;
            case ValueType.UInt:
                PluginLog.Debug($"[{index:D3}] [{"uint", 7}]: {value.UInt}");
                break;
            case ValueType.Float:
                PluginLog.Debug($"[{index:D3}] [{"float", 7}]: {value.Float}");
                break;
            case ValueType.String:
                PluginLog.Debug($"[{index:D3}] [{"string", 7}]: {Marshal.PtrToStringUTF8(new IntPtr(value.String))}");
                break;
            case ValueType.String8:
                PluginLog.Debug($"[{index:D3}] [{"string8", 7}]: {Marshal.PtrToStringUTF8(new IntPtr(value.String))}");
                break;
            case ValueType.Vector:
                break;
            case ValueType.AllocatedString:
                PluginLog.Debug($"[{index:D3}] [{"aString", 7}]: {Marshal.PtrToStringUTF8(new IntPtr(value.String))}");
                break;
            case ValueType.AllocatedVector:
                break;
            default:                        
                PluginLog.Debug($"[{index:D3}] [{"unknown", 7}]: Type: [{value.Type}], Value: {value.Int}");
                break;
        }
    }
}