using static Osmium.Interpreter.Interpreter;
using static Osmium.Interpreter.OsmiumParser;

namespace Osmium.Interpreter;

public class Function
{
    public string? Identifier { get; set; }
    public Program_blockContext program { get; set; }
    public string[] param_list { get; set; }

    public Function(string? ident, Program_blockContext program, string[] param_list)
    {
        Identifier = ident;
        this.program = program;
        this.param_list = param_list;
    }

    public object? Invoke(Interpreter visitor, object[] args)
    {
        var argCount = args.Length;
        var paramCount = param_list.Length;

        if (argCount != paramCount)
        {
            throw new ArgumentException($"Invalid parameter count. Expected: {paramCount}, Given: {argCount}");
        }

        // func should know about itself
        if (Identifier is not null)
            visitor.SymbolTable[Identifier] = this;
        visitor.SymbolTable["this"] = this;

        // set param symbols based on param_list identifiers and args objects
        for (int i = 0; i < argCount; i++)
        {
            var symbol = param_list[i];
            if (visitor.SymbolTable.HasSymbol(symbol))
                throw new InvalidOperationException($"local param {symbol} already defined ;(");

            visitor.SymbolTable[symbol] = args[i];
        }

        try
        {
            // visit program
            visitor.VisitProgram_block(program); ;
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
        return $"{base.ToString()}:[{(Identifier is null ? "Lambda" : Identifier)}] ParamCount:[{param_list.Length}]";
    }
}
