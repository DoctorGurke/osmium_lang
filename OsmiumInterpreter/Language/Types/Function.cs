using Antlr4.Runtime;

namespace Osmium.Language.Types;

public class Function : IFunction
{
    public string? Identifier { get; set; }
    private ParserRuleContext Program { get; set; }
    public string[] Parameters { get; set; }

    public Function(string? ident, ParserRuleContext program, string[] param_list)
    {
        Identifier = ident;
        Program = program;
        Parameters = param_list;
    }

    public object? Invoke(Interpreter visitor, object[] args)
    {
        var argCount = args.Length;
        var paramCount = Parameters.Length;

        if (argCount != paramCount)
        {
            throw new ArgumentException($"Invalid parameter count. Expected: {paramCount}, Given: {argCount}");
        }

        // func should know about itself
        if (Identifier is not null)
            visitor.Members.SetSymbol(Identifier, this);

        visitor.Members.SetSymbol("this", this);

        // set param symbols based on param_list identifiers and args objects
        for (int i = 0; i < argCount; i++)
        {
            var symbol = Parameters[i];
            visitor.Members.SetSymbol(symbol, args[i]);
        }

        try
        {
            // visit program
            visitor.Visit(Program); ;
        }
        catch (ReturnException returnEx)
        {
            return returnEx.Value;
        }

        // void return
        return null;
    }

    public override string ToString()
    {
        return $"Function:[{(Identifier is null ? "Lambda" : $"Identifier:{Identifier}")}] ParamCount:[{Parameters.Length}]";
    }
}
