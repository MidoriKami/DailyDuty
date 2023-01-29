using System.Collections.Generic;
using DailyDuty.Interfaces;
using KamiLib;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;
using SignatureHelper = Dalamud.Utility.Signatures.SignatureHelper;

namespace DailyDuty.Modules;

// ReSharper disable once RedundantUnsafeContext
public unsafe class TestModule : AbstractTestModule
{
    public TestModule()
    {
        SignatureHelper.Initialise(this);

        KamiCommon.CommandManager.AddCommand(new TestCommand());
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
            CommandAction = TestModule.DoTheThing
        }
    };
}