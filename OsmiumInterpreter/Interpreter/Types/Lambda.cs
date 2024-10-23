using static Osmium.Interpreter.OsmiumParser;

namespace Osmium.Interpreter.Types;

public class Lambda : IFunction
{
    public ExpressionContext expression { get; set; }
    public string[] param_list { get; set; }

    public Lambda(ExpressionContext expression, string[] param_list)
    {
        this.expression = expression;
        this.param_list = param_list;
    }

    public object Invoke(Interpreter visitor, object[] args)
    {
        var argCount = args.Length;
        var paramCount = param_list.Length;

        if (argCount != paramCount)
        {
            throw new ArgumentException($"Invalid parameter count.");
        }

        visitor.SymbolTable.SetSymbol("this", this);

        // set param symbols based on param_list identifiers and args objects
        for (int i = 0; i < argCount; i++)
        {
            var symbol = param_list[i];
            if (visitor.SymbolTable.HasSymbol(symbol))
                throw new InvalidOperationException($"local param {symbol} already defined ;(");

            visitor.SymbolTable.SetSymbol(symbol, args[i]);
        }

        // evaluate expression
        return visitor.VisitExpression(expression);
    }
}
