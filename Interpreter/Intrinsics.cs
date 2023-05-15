using System.Text;

namespace Osmium.Interpreter;

public static class Intrinsics
{
    public static void Print(List<object> args)
    {
        if (args is null || args.Count != 1)
            return;

        var parameter = args[0];
        string printString = parameter?.ToString();

        // list parameter
        if (parameter is List<object> list)
        {
            var sb = new StringBuilder();

            foreach (var item in list)
            {
                sb.Append($"{item}, ");
            }

            printString = sb.ToString().Trim().Trim(',');
        }

        Log.Info($"{printString}");
    }

    public static int Length(List<object> args)
    {
        if (args is null || args.Count != 1)
            return 0;

        if (args[0] is not IList<object> collection)
            return 0;

        return collection.Count;
    }

    public static void ForEach(List<object> args)
    {
        if (args is null || args.Count != 2)
            return;

        if (args[0] is not IList<object> collection || args[1] is not Lambda lambda)
            return;

        foreach (var item in collection)
        {
            Log.Info($"foreach: {item}");
            lambda.Invoke(new Interpreter(), new object[] { item });
        }
    }
}
