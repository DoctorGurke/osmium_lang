using System.Collections.Generic;

namespace Osmium.Interpreter;

public class SymbolTable
{
    public bool IsRoot => Parent is null;

    public SymbolTable Parent { get; set; }

    public Dictionary<string, object> Symbols { get; set; } = new Dictionary<string, object>();

    public SymbolTable(SymbolTable parent = null)
    {
        Parent = parent;
    }

    public object this[string symbol]
    {
        get => Symbols[symbol];
        set => Symbols[symbol] = value;
    }

    public bool HasSymbol(string symbol)
    {
        if (!IsRoot)
        {
            return Parent.HasSymbol(symbol) || Symbols.ContainsKey(symbol);
        }

        return Symbols.ContainsKey(symbol);
    }

    public bool TryGetValue(string symbol, out object value)
    {
        if (!IsRoot)
        {
            if (Parent.TryGetValue(symbol, out value))
            {
                return true;
            }
        }

        if (!HasSymbol(symbol))
        {
            value = null;
            return false;
        }

        value = Symbols[symbol];
        return true;
    }
}
