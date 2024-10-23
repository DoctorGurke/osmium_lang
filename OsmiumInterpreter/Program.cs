global using System;
global using System.Collections.Generic;
using Osmium.Interpreter;

namespace Osmium;

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
