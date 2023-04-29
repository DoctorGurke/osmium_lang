using System;

namespace OsmiumLang;

public class Program
{
    static void Main(string[] args)
    {
        // todo: runtime lol

        while (true)
        {
            var input = System.Console.ReadLine();
            if (input is null)
                continue;

            if (input.StartsWith("run "))
            {
                var script = input.Substring(4);
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
        System.Console.WriteLine($"{content}");
    }
}

public class ScriptNotFoundException : Exception
{
    public ScriptNotFoundException(string message) : base(message) { }
}
