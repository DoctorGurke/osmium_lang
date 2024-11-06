namespace Osmium;

public static class Log
{
    /// <summary>
    /// Used for general console output.
    /// </summary>
    /// <param name="message"></param>
    public static void Print(object message)
    {
        Console.WriteLine($"> {message}");
    }

    /// <summary>
    /// Used for debug logging. Silenced by default.
    /// </summary>
    /// <param name="content"></param>
    public static void Debug(object content)
    {
        Console.WriteLine($"{content}");
    }
}
