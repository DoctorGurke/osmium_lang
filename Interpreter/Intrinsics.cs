using System.Text;

namespace Osmium.Interpreter;

public static class Intrinsics
{
    public static void Print(List<object> args)
    {
        if (args is null || args.Count != 1)
            throw new ArgumentException($"Invalid arg count for intrinsic Print. Expected: object");

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
            throw new ArgumentException($"Invalid arg count for intrinsic Length. Expected: list");

        if (args[0] is not IList<object> collection)
            throw new ArgumentException($"Invalid arg types for intrinsic Length. Expected: list. Got: {args[0].GetType()}");

        return collection.Count;
    }

    public static void ForEach(List<object> args)
    {
        if (args is null || args.Count != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic ForEach. Expected: list, lambda");

        if (args[0] is not IList<object> collection || args[1] is not Lambda lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic ForEach. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        foreach (var item in collection)
        {
            lambda.Invoke(new Interpreter(), new object[] { item });
        }
    }

    public static List<object> Map(List<object> args)
    {
        if (args is null || args.Count != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic Map. Expected: list, lambda");

        if (args[0] is not IList<object> collection || args[1] is not Lambda lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic Map. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        var returnCollection = new List<object>();

        foreach (var item in collection)
        {
            var eval = lambda.Invoke(new Interpreter(), new object[] { item });
            returnCollection.Add(eval);
        }

        return returnCollection;
    }
}
