using System;
using System.Diagnostics;
using System.Reflection;
using Dalamud.Logging;

namespace DailyDuty.Utilities;

internal static class Log
{
    public static void Verbose(string message)
    {
        var callingAssembly = NameOfCallingClass();

        PluginLog.Verbose($"[{callingAssembly}] {message}");
    }

    public static string? NameOfCallingClass()
    {
        string? fullName;
        Type? declaringType;
        var skipFrames = 2;

        do
        {
            MethodBase? method = new StackFrame(skipFrames, false).GetMethod();
            declaringType = method?.DeclaringType;
            if (declaringType == null)
            {
                return method?.Name;
            }
            skipFrames++;
            fullName = declaringType.Name;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return fullName;
    }
}