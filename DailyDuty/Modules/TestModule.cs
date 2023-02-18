using System.Collections.Generic;
using DailyDuty.Interfaces;
using Dalamud.Utility.Signatures;
using KamiLib;
using KamiLib.ChatCommands;
using KamiLib.Hooking;
using KamiLib.Interfaces;

namespace DailyDuty.Modules;

// ReSharper disable once RedundantUnsafeContext
public unsafe class TestModule : AbstractTestModule
{
    public TestModule()
    {
        SignatureHelper.Initialise(this);

        KamiCommon.CommandManager.AddCommand(new TestCommand());
    }

    public override void Dispose()
    {
    }

    public static void DoTheThing()
    {
        
    }
}

public class TestCommand : IPluginCommand
{
    public string CommandArgument => "test";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            Hidden = true,
            CommandAction = () => Safety.ExecuteSafe(TestModule.DoTheThing)
        }
    };
}