using NUnit.Framework;
using Osmium.Language;

namespace Osmium.Tests;

[TestFixture]
public class TestSymbolTable
{
    private SymbolTable? symbolTable;

    [SetUp]
    public void SetUp()
    {
        symbolTable = null;
        symbolTable = new SymbolTable();
    }

    [Test]
    public void TestSetGetSymbol()
    {
        // given an empty symbol table an identifier and a value
        if (symbolTable is null)
            return;
        var identifier = "var";
        var value = 1000;

        // when setting a symbol
        symbolTable.SetSymbol(identifier, value);

        // then the value should match when getting the symbol back from the table
        Assert.That(symbolTable.GetSymbolValue(identifier), Is.EqualTo(value));
    }

    [Test]
    public void TestSetSymbolRedefinition()
    {
        // given an empty symbol table and an identifier
        if (symbolTable is null)
            return;
        var identifier = "var";

        // when setting a symbol
        symbolTable.SetSymbol(identifier, 0);

        // then setting the same symbol again should throw
        Assert.Throws<RuntimeException>(() => { symbolTable.SetSymbol(identifier, 0); });
    }

    [Test]
    public void TestGetUndefinedSymbol()
    {
        // given an empty symbol table and an identifier
        if (symbolTable is null)
            return;

        // when setting any other symbol
        symbolTable.SetSymbol("var", 0);

        // then getting an undefined symbol should throw
        Assert.Throws<RuntimeException>(() => { symbolTable.GetSymbolValue("value"); });
    }

    [Test]
    public void TestHasSymbol()
    {
        // given an empty symbol table and an identifier
        if (symbolTable is null)
            return;
        var identifier = "var";

        // when setting a symbol
        symbolTable.SetSymbol(identifier, 0);

        // then HasSymbol should be true for the same identifier
        Assert.That(symbolTable.HasSymbol(identifier), Is.True);
    }

    [Test]
    public void TestHasUndefinedSymbol()
    {
        // given an empty symbol table
        if (symbolTable is null)
            return;

        // when setting any other symbol
        symbolTable.SetSymbol("var", 0);

        // then HasSymbol should be false for another identifier
        Assert.That(symbolTable.HasSymbol("value"), Is.False);
    }

    [Test]
    public void TestParentHasSymbol()
    {
        // given a symbol table with a symbol as a parent
        if (symbolTable is null)
            return;
        symbolTable.SetSymbol("var", 1);

        // when creating a child symbol table
        var child = new SymbolTable(symbolTable);

        // then the child HasSymbol should be true for the parent symbol
        Assert.That(child.HasSymbol("var"), Is.True);
    }

    [Test]
    public void TestTryGetSymbol()
    {
        // given an empty symbol table, an identifier and a value
        if (symbolTable is null)
            return;
        var identifier = "var";
        var value = 1;

        // when adding a symbol with the value
        symbolTable.SetSymbol(identifier, value);

        // then TryGetSymbol should get the idenfitier
        Assert.That(symbolTable.TryGetSymbol(value, out var symbolOut), Is.True);
        Assert.That(symbolOut, Is.EqualTo(identifier));
    }
}
