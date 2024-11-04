namespace TestProject.Syntax;

[TestFixture]
public class Expressions : OsmiumTestRunner
{
    [TestCase("result = 5 + (3 + 3) - 4;", 7)]
    [TestCase("x = 1; result = x * 10 + (1 + x);", 12)]
    [TestCase("x = [1]; result = x[0] + 10;", 11)]
    [TestCase("function x: return 1 end; result = x() + 10;", 11)]
    [TestCase("enum x = [state1, state2]; result = x.state2 + 10;", 11)]
    public void VerifyExpressions(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("result = 1 * 3;", 3)]
    [TestCase("result = 10 / 5;", 2)]
    [TestCase("result = 17 % 2;", 1)]
    [TestCase("result = 3 + 1;", 4)]
    [TestCase("result = 4 - 1;", 3)]
    public void VerifyArithmeticExpression(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
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
        Assert.Throws<ParserException>(() => { Runtime!.Run(input, local: true); });
    }

    [TestCase("result = !true;", false)]
    [TestCase("result = 5 > 3;", true)]
    [TestCase("result = 10 >= 10;", true)]
    [TestCase("result = 1 <= 3;", true)]
    [TestCase("result = 5 != 1;", true)]
    [TestCase("result = 1 == 1;", true)]
    [TestCase("result = true && true;", true)]
    [TestCase("result = true || false;", true)]
    public void VerifyBooleanExpressions(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
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
        Assert.Throws<ParserException>(() => { Runtime!.Run(input, local: true); });
    }

    [TestCase("result = 2 + 3 * 5;", 17)]
    [TestCase("result = (2 + 3) * 5;", 25)]
    [TestCase("result = 1 + 1 + 1 + 1 + 1 + 1 * 5;", 10)]
    [TestCase("result = (1 + 1 + 1 + 1 + 1 + 1) * 5;", 30)]
    [TestCase("result = -3 * 10 - 5;", -35)]
    public void VerifyOperatorPriority(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }
}
