using Osmium.Interpreter;

namespace Osmium;

public class Program
{
    public static void Main(string[] args)
    {
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
                    runtime.Run(Script.Load(script), true);
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
}