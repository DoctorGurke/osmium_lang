using System.Diagnostics.CodeAnalysis;

namespace Osmium.Interpreter;

public class SymbolTable : IMembers
{
    public SymbolTable Members => this;
    public bool IsRoot => Parent is null;

    public SymbolTable? Parent { get; set; }

    private Dictionary<string, object> Symbols { get; set; } = new Dictionary<string, object>();

    public SymbolTable(SymbolTable? parent = null)
    {
        Parent = parent;
    }

    public void SetSymbol(string symbol, object value)
    {
        if (HasSymbol(symbol))
            throw new Exception($"Trying to redefine immutable symbol '{symbol}'!");
        Symbols[symbol] = value;
    }

    public object GetSymbol(string symbol)
    {
        if (Symbols.TryGetValue(symbol, out var value)) 
            return value;
        throw new Exception($"Trying to access undeclared symbol '{symbol}'!");
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

    /// <summary>
    /// Try get a symbol name from a value. 
    /// </summary>
    /// <param name="value">Value to look for.</param>
    /// <param name="symbol">First declared symbol of value.</param>
    /// <returns>True - Symbol found. False - No Symbol found</returns>
    public bool TryGetSymbol(object value, [NotNullWhen(returnValue: true)]out string? symbol)
    {
        foreach (var member in Symbols)
        {
            if (Equals(member.Value, value))
            {
                symbol = member.Key;
                return true;
            }
        }
        symbol = null;
        return false;
    }
}
