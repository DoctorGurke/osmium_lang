﻿using System.Diagnostics.CodeAnalysis;

namespace Osmium.Interpreter;

public class SymbolTable : IMembers
{
    public SymbolTable Members => this;
    public bool IsRoot => Parent is null;

    public SymbolTable? Parent { get; set; }

    public Dictionary<string, object> Symbols { get; set; } = new Dictionary<string, object>();


    public SymbolTable(SymbolTable? parent = null)
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
        if (!IsRoot && Parent is not null)
        {
            return Parent?.HasSymbol(symbol) ?? false || Symbols.ContainsKey(symbol);
        }

        return Symbols.ContainsKey(symbol);
    }

    public bool TryGetValue(string symbol, [NotNullWhen(returnValue: true)] out object? value)
    {
        if (!IsRoot && Parent is not null)
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
