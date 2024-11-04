namespace TestProject.Syntax;

[TestFixture]
public class Scopes
{
    private Runtime? runtime;

    [SetUp]
    public void SetUp()
    {
        runtime = null;
        runtime = new Runtime();
    }

    [Test]
    public void TestParentSymbols()
    {
        // given a script that validates a global symbol from a scope
        var input = """
            global = 0;
            {
                assert(global == 0);
            };
            """;

        // when running the code it should not throw from the assert
        Assert.DoesNotThrow(() => runtime!.Run(input));

        // then the global symbol should be in the symbol table
        Assert.That(runtime!.Members.GetSymbolValue("global"), Is.EqualTo(0));
    }

    [Test]
    public void TestLocalSymbol()
    {
        // given a script that defines a local symbol in a scope
        var input = """
            {
                x = 1;
            };
            """;

        // when running the code
        Assert.DoesNotThrow(() => runtime!.Run(input));

        // then the symbol should not be defined
        Assert.That(runtime!.Members.HasSymbol("x"), Is.False);
    }
}
