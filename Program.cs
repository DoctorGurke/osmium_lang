global using System;
global using System.Collections.Generic;
using Osmium.Interpreter;

namespace Osmium;

public class Program
{
    static void Main()
    {
        var runtime = new Runtime();

        while (true)
        {
            var input = Console.ReadLine();
            if (input is null)
                continue;

            if (input.StartsWith("run "))
            {
                var script = input[4..];
                Log.Info($"running {script}");
                runtime.RunLocal(Script.Load(script));
                continue;
            }
            else if (input.StartsWith("include "))
            {
                var script = input[8..];
                Log.Info($"including {script}");
                runtime.Run(Script.Load(script));
                continue;
            }

            runtime.Run(input);
        }
    }
}

public static class Log
{
    public static void Space()
    {
        Info(">");
    }

    public static void Info(object content)
    {
        Console.WriteLine($"{content}");
    }
}