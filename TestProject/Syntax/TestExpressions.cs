using Osmium.Interpreter;

namespace TestProject.Syntax;

[TestFixture]
public class TestExpressions
{
    private Osmium.Interpreter.Runtime? runtime;

    [SetUp]
    public void SetUp()
    {
        runtime = new Osmium.Interpreter.Runtime();
    }

    [TestCase("x = !true;")]
    [TestCase("x = 5 > 3;")]
    [TestCase("x = 10 >= 10;")]
    [TestCase("x = 1 <= 3;")]
    [TestCase("x = 5 != 1;")]
    [TestCase("x = 1 == 1;")]
    [TestCase("x = true && true;")]
    [TestCase("x = true || false;")]
    public void VerifyBooleanExpressions(string input)
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input, local: true); });
    }

    [TestCase("x = !;")]
    [TestCase("x = > 5;")]
    [TestCase("x = 5 >;")]
    [TestCase("x = 10 >=;")]
    [TestCase("x = >= 10;")]
    [TestCase("x = <= 1;")]
    [TestCase("x = 1 <=;")]
    [TestCase("x = != 1;")]
    [TestCase("x = 1 !=;")]
    [TestCase("x = == 0;")]
    [TestCase("x = 0 ==;")]
    [TestCase("x = && true;")]
    [TestCase("x = true &&;")]
    [TestCase("x = || false;")]
    [TestCase("x = true ||;")]
    public void VerifyInvalidBooleanExpressions(string input)
    {
        Assert.Throws<ParserException>(() => { runtime!.Run(input, local: true); });
    }
}
