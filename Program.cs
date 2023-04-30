using System;

namespace Osmium;

public class Program
{
    static void Main()
    {
        // todo: runtime lol

        while (true)
        {
            var input = Console.ReadLine();
            if (input is null)
                continue;

            if (input.StartsWith("run "))
            {
                var script = input[4..];
                Log.Info($"running {script}");
                Test.TryScript(script);
                continue;
            }

            Test.Try(input);
        }
    }
}

public static class Log
{
    public static void Info(object content)
    {
        Console.WriteLine($"{content}");
    }
}

public class ScriptNotFoundException : Exception
{
    public ScriptNotFoundException(string message) : base(message) { }
}
