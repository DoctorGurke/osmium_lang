global using System;
using Osmium.Interpreter;

namespace Osmium;

public class Program
{
    static void Main()
    {
        // todo: runtime lol
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
                //Test.TryScript(script);
                runtime.RunLocal(Script.Load(script));
                continue;
            }

            runtime.Run(input);
            //runtime.Evaluate(input);
            //Test.Try(input);
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