using Osmium.Language.Types;
using System.Text;

namespace Osmium.Language;

public static class Intrinsics
{
    public static void Print(object[] args)
    {
        if (args is null || args.Length != 1)
            throw new ArgumentException($"Invalid arg count for intrinsic Print. Expected: object");

        var parameter = args[0];
        string printString = $"{parameter}";

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

        Console.WriteLine($"{printString}");
    }

    public static int Length(object[] args)
    {
        if (args is null || args.Length != 1)
            throw new ArgumentException($"Invalid arg count for intrinsic Length. Expected: list");

        if (args[0] is not IList<object> collection)
            throw new ArgumentException($"Invalid arg types for intrinsic Length. Expected: list. Got: {args[0].GetType()}");

        return collection.Count;
    }

    public static void ForEach(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic ForEach. Expected: list, lambda");

        if (args[0] is not IList<object> collection || args[1] is not IFunction lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic ForEach. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        foreach (var item in collection)
        {
            lambda.Invoke(new Interpreter(), new object[] { item });
        }
    }

    public static List<object?> Map(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic Map. Expected: list, lambda");

        if (args[0] is not IList<object> collection || args[1] is not IFunction lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic Map. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        var returnCollection = new List<object?>();

        foreach (var item in collection)
        {
            var eval = lambda.Invoke(new Interpreter(), new object[] { item });
            returnCollection.Add(eval);
        }

        return returnCollection;
    }

    public static object Reduce(object[] args)
    {
        if (args is null || args.Length != 3)
            throw new ArgumentException($"Invalid arg count for intrinsic Reduce. Expected: list, lambda, object");

        if (args[0] is not IList<object> collection || args[1] is not IFunction lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic Reduce. Expected: list, lambda, object. Got: {args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}");

        var acc = args[2];

        foreach (var item in collection)
        {
            acc = lambda.Invoke(new Interpreter(), new object[] { acc, item });
        }

        return acc;
    }

    public static List<object> Filter(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic Filter. Expected: list, lambda");

        if (args[0] is not IList<object> collection || args[1] is not IFunction lambda)
            throw new ArgumentException($"Invalid arg types for intrinsic Filter. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        var returnCollection = new List<object>();

        foreach (var item in collection)
        {
            var cond = (bool)(lambda.Invoke(new Interpreter(), new object[] { item }) ?? false);
            if (cond)
                returnCollection.Add(item);
        }

        return returnCollection;
    }

    public class AssertException : Exception
    {
        public AssertException(string message) : base(message) { }
    }

    public static void Assert(object[] args)
    {
        if (args.Length < 1)
            throw new ArgumentException($"Invalid arg count for assert. Expected 2: condition, optional:message. Got {args.Length}.");

        if (args[0] is not bool eval)
            throw new ArgumentException($"Invalid arg types for assert. Expected condition. Got: {args[0].GetType()}");

        if (eval)
            return;

        throw new AssertException((args.Length == 2) ? $"{args[1]}" : "");
    }
}
