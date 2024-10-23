using Osmium.Interpreter;

namespace Osmium;

public class ConsoleRunner : IMembers
{
    public static void Main(string[] _)
    {
        var runtime = new Runtime();
        Instance = new ConsoleRunner(runtime);
        Instance.StartRuntime();
    }

    public static ConsoleRunner? Instance { get; private set; }

    public SymbolTable Members => runtime.Members;

    private bool exit;
    private readonly Runtime runtime;

    public ConsoleRunner(Runtime runtime)
    {
        this.runtime = runtime;
    }

    /// <summary>
    /// Start a Console input listening loop. Processes input per line.
    /// </summary>
    public void StartRuntime()
    {
        while (!exit)
        {
            var input = Console.ReadLine();
            if (input is null)
                continue;

            ProcessInput(input);
        }

        exit = false;
    }

    /// <summary>
    /// Process input command or expression. 
    /// Handles Exit, Run(Local execute), Include(load script file) and direct osmium expression.
    /// </summary>
    /// <param name="input">Input command or expression</param>
    public void ProcessInput(string input)
    {
        switch (input)
        {
            case var x when x.StartsWith("exit"):
                exit = true;
                break;

            case var x when x.StartsWith("run "):
                var script = input[4..];
                Log.Info($"running {script}");
                runtime.Run(Script.Load(script), local: true);
                break;

            case var x when x.StartsWith("include "):
                var includeScript = input[8..];
                Log.Info($"including {includeScript}");
                runtime.Run(Script.Load(includeScript));
                break;

        }

        runtime.Run(input);
    }
}