﻿namespace Osmium.Language.Types;

public class Enum : IMembers
{
    public string Name { get; private set; }
    public SymbolTable Members { get; private set; } = new();

    /// <summary>
    /// Construct an enum declaration from name and string member names with optional int value. 
    /// Value is generated if not specified.
    /// Verifies member declarations, throws on explicit redefinition.
    /// </summary>
    /// <param name="name">Enum type name.</param>
    /// <param name="members">Enum members.</param>
    public Enum(string name, Dictionary<string, int?> members)
    {
        Name = name;

        if (!members.Any())
            throw new ArgumentException($"Invalid enum declaration {name} with 0 members!");

        int defaultValue = 0;
        foreach (var member in members)
        {
            int value = defaultValue;
            if (member.Value is int val)
            {
                value = val;
            }

            if (value < 0)
                throw new ArgumentException($"Invalid negative value for enum member {name}.{member.Key}!");

            Members.SetSymbol(member.Key, value);

            if (value == defaultValue)
                defaultValue++;
        }
    }

    public string GetIndexOf(int value)
    {
        if (Members.TryGetSymbol(value, out var symbol))
            return symbol;

        throw new Exception($"Trying to get null state {Name}[{value}]!");
    }

    public override string ToString()
    {
        return $"Enum:[Identifier:{Name}] Members:[{Members}]";
    }
}
