global using System;
global using System.Collections.Generic;

namespace Osmium;

public static class Log
{
    public static void Space()
    {
        Info(">");
    }

    public static void Info(object content)
    {
#if CI_TEST
        return;
#endif
        Console.WriteLine($"{content}");
    }
}
