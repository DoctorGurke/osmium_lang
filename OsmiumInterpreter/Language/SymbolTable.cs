using System.Diagnostics.CodeAnalysis;

namespace Osmium.Language;

public class SymbolTable : IMembers
{
    public SymbolTable Members => this;
    private bool IsRoot => Parent is null;

    private SymbolTable? Parent { get; set; }

    private Dictionary<string, object> Symbols { get; set; } = new Dictionary<string, object>();

    public SymbolTable(SymbolTable? parent = null)
    {
        Parent = parent;
    }

    /// <summary>
    /// Set a symbol
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value"></param>
    /// <exception cref="Exception"></exception>
    public void SetSymbol(string symbol, object value)
    {
        if (HasSymbol(symbol))
            throw new Exception($"Trying to redefine immutable symbol '{symbol}'!");
        Symbols[symbol] = value;
    }

    /// <summary>
    /// Get a value from the symbol table. Throws if symbol is undeclared.
    /// </summary>
    /// <param name="symbol">Symbol to check for.</param>
    /// <returns>Value of symbol in symbol table.</returns>
    /// <exception cref="Exception"></exception>
    public object GetSymbolValue(string symbol)
    {
        if (TryGetValue(symbol, out var value))
            return value;
        throw new Exception($"Trying to access undeclared symbol '{symbol}'!");
    }

    /// <summary>
    /// Checks if the symbol table (or its parents) contains a value for a symbol.
    /// </summary>
    /// <param name="symbol">Symbol to check.</param>
    /// <returns>True - Table contains symbol :: False - Table does not contain symbol.</returns>
    public bool HasSymbol(string symbol)
    {
        if (!IsRoot && Parent is not null)
        {
            return Parent.HasSymbol(symbol) || Symbols.ContainsKey(symbol);
        }

        return Symbols.ContainsKey(symbol);
    }

    /// <summary>
    /// Try get a value from a symbol.
    /// </summary>
    /// <param name="symbol">Symbol to get the value from.</param>
    /// <param name="value">Returned value.</param>
    /// <returns>True - Symbol found :: False - Symbol not found.</returns>
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
    public bool TryGetSymbol(object value, [NotNullWhen(returnValue: true)] out string? symbol)
    {
        // used for enum indexing, no need to check parent tables
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
