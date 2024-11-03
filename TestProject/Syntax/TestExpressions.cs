using Osmium.Interpreter;

namespace TestProject.Syntax;

[TestFixture]
public class TestExpressions
{
    private Runtime? runtime;

    [SetUp]
    public void SetUp()
    {
        runtime = new Runtime();
    }

    [TestCase("x = 5 + (3 + 3) - 4;")]
    [TestCase("x = 1; y = x * 10 + (1 + x);")]
    [TestCase("x = [1]; y = x[0] + 10;")]
    [TestCase("function x: return 1 end; y = x() + 10;")]
    [TestCase("enum x = [state1]; y = x.state1 + 10;")]
    public void VerifyExpressions(string input)
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input, local: true); });
    }

    [TestCase("x = 1 * 3;")]
    [TestCase("x = 10 / 5;")]
    [TestCase("x = 17 % 2;")]
    [TestCase("x = 3 + 1;")]
    [TestCase("x = 4 - 1;")]
    public void VerifyArithmeticExpression(string input)
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input, local: true); });
    }

    [TestCase("x = * 1;")]
    [TestCase("x = 1 *;")]
    [TestCase("x = / 5;")]
    [TestCase("x = 5 /;")]
    [TestCase("x = % 1;")]
    [TestCase("x = 1 %;")]
    [TestCase("x = 3 +;")]
    [TestCase("x = 4 -;")]
    public void VerifyInvalidArithmeticExpression(string input)
    {
        Assert.Throws<ParserException>(() => { runtime!.Run(input, local: true); });
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
