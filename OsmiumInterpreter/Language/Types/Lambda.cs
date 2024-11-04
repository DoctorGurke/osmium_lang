using Antlr4.Runtime;

namespace Osmium.Language.Types;

public class Lambda : IFunction
{
    private ParserRuleContext Expression { get; set; }
    public string[] Parameters { get; set; }

    public Lambda(ParserRuleContext expression, string[] param_list)
    {
        Expression = expression;
        Parameters = param_list;
    }

    public object Invoke(Interpreter visitor, object[] args)
    {
        var argCount = args.Length;
        var paramCount = Parameters.Length;

        if (argCount != paramCount)
        {
            throw new ArgumentException($"Invalid parameter count.");
        }

        visitor.Members.SetSymbol("this", this);

        // set param symbols based on param_list identifiers and args objects
        for (int i = 0; i < argCount; i++)
        {
            var symbol = Parameters[i];
            if (visitor.Members.HasSymbol(symbol))
                throw new InvalidOperationException($"local param {symbol} already defined ;(");

            visitor.Members.SetSymbol(symbol, args[i]);
        }

        // evaluate expression
        return visitor.Visit(Expression);
    }
}
