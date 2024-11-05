using Osmium.Language.Types;
using System.Text;

namespace Osmium.Language;

[AttributeUsage(AttributeTargets.Method)]
public class IntrinsicFunctionAttribute : Attribute
{
    public string Identifier { get; private set; }
    public string[] Parameters { get; private set; }
    public IntrinsicFunctionAttribute(string identifier, params string[] parameters)
    {
        Identifier = identifier;
        Parameters = parameters;
    }
}

public static class Intrinsics
{
    [IntrinsicFunction(identifier: "print", "message")]
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

    [IntrinsicFunction(identifier: "length", "list")]
    public static int Length(object[] args)
    {
        if (args is null || args.Length != 1)
            throw new ArgumentException($"Invalid arg count for intrinsic Length. Expected: list");

        if (args[0] is not IList<object> list)
            throw new ArgumentException($"Invalid arg types for intrinsic Length. Expected: list. Got: {args[0].GetType()}");

        return list.Count;
    }

    [IntrinsicFunction(identifier: "foreach", "list", "function")]
    public static void ForEach(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic ForEach. Expected: list, lambda");

        if (args[0] is not IList<object> list || args[1] is not IFunction function)
            throw new ArgumentException($"Invalid arg types for intrinsic ForEach. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        foreach (var item in list)
        {
            var visitor = new Interpreter();
            function.Invoke(visitor, new object[] { item });
        }
    }

    [IntrinsicFunction(identifier: "map", "list", "function")]
    public static List<object?> Map(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic Map. Expected: list, lambda");

        if (args[0] is not IList<object> list || args[1] is not IFunction function)
            throw new ArgumentException($"Invalid arg types for intrinsic Map. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        var returnCollection = new List<object?>();

        foreach (var item in list)
        {
            var visitor = new Interpreter();
            var eval = function.Invoke(visitor, new object[] { item });
            returnCollection.Add(eval);
        }

        return returnCollection;
    }

    [IntrinsicFunction(identifier: "reduce", "list", "function")]
    public static object Reduce(object[] args)
    {
        if (args is null || args.Length != 3)
            throw new ArgumentException($"Invalid arg count for intrinsic Reduce. Expected: list, lambda, object");

        if (args[0] is not IList<object> list || args[1] is not IFunction function)
            throw new ArgumentException($"Invalid arg types for intrinsic Reduce. Expected: list, lambda, object. Got: {args[0].GetType()}, {args[1].GetType()}, {args[2].GetType()}");

        var acc = args[2];

        foreach (var item in list)
        {
            var visitor = new Interpreter();
            acc = function.Invoke(visitor, new object[] { acc, item });
        }

        return acc;
    }

    [IntrinsicFunction(identifier: "filter", "list", "function")]
    public static List<object> Filter(object[] args)
    {
        if (args is null || args.Length != 2)
            throw new ArgumentException($"Invalid arg count for intrinsic Filter. Expected: list, lambda");

        if (args[0] is not IList<object> list || args[1] is not IFunction function)
            throw new ArgumentException($"Invalid arg types for intrinsic Filter. Expected: list, lambda. Got: {args[0].GetType()}, {args[1].GetType()}");

        var returnCollection = new List<object>();

        foreach (var item in list)
        {
            var visitor = new Interpreter();
            var cond = (bool)(function.Invoke(visitor, new object[] { item }) ?? false);
            if (cond)
                returnCollection.Add(item);
        }

        return returnCollection;
    }

    [IntrinsicFunction(identifier: "assert", "condition", "message?")]
    public static void Assert(object[] args)
    {
        if (args.Length < 1)
            throw new ArgumentException($"Invalid arg count for assert. Expected 2: condition, optional:message. Got {args.Length}.");

        if (args[0] is not bool condition)
            throw new ArgumentException($"Invalid arg types for assert. Expected condition. Got: {args[0].GetType()}");

        if (condition)
            return;

        throw new AssertException((args.Length == 2) ? $"{args[1]}" : "");
    }
}
