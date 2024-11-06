namespace OsmiumRuntime;

public class ConsoleRunner : IMembers
{
    public static void Main(string[] args)
    {
        var debug = false;
        if (args.Length > 0 && args[0] == "-debug")
        {
            debug = true;
            Log.Info($"Debug Enabled.");
        }
        var runtime = new Runtime(debug: debug);
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
                return;

            case var x when x.StartsWith("run "):
                var script = input[4..];
                Log.Print($"running {script}");
                runtime.Run(Script.Load(script), local: true);
                return;

            case var x when x.StartsWith("include "):
                var includeScript = input[8..];
                Log.Print($"including {includeScript}");
                runtime.Run(Script.Load(includeScript));
                return;

        }

        runtime.Run(input);
    }
}