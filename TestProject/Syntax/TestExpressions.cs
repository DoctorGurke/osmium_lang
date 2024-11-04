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

    [TestCase("result = 5 + (3 + 3) - 4;", 7)]
    [TestCase("x = 1; result = x * 10 + (1 + x);", 12)]
    [TestCase("x = [1]; result = x[0] + 10;", 11)]
    [TestCase("function x: return 1 end; result = x() + 10;", 11)]
    [TestCase("enum x = [state1, state2]; result = x.state2 + 10;", 11)]
    public void VerifyExpressions(string input, object result)
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input); });
        Assert.That(result, Is.EqualTo(runtime!.Members.GetSymbolValue("result")));
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
