namespace Osmium;

public static class Log
{
    /// <summary>
    /// Used for general console output.
    /// </summary>
    /// <param name="message"></param>
    public static void Print(object message)
    {
        Console.WriteLine($">{message}");
    }

    /// <summary>
    /// Used for debug logging. Silenced by default.
    /// </summary>
    public static void Space()
    {
        Info(">");
    }

    /// <summary>
    /// Used for debug logging. Silenced by default.
    /// </summary>
    /// <param name="content"></param>
    public static void Info(object content)
    {
        Console.WriteLine($"{content}");
    }
}
