namespace TestProject.Syntax;

[TestFixture]
public class Functions : OsmiumTestRunner
{
    [TestCase("function x: end;")]
    [TestCase("function x(): end;")]
    [TestCase("function x(x): end;")]
    [TestCase("function x(x,y): end;")]
    [TestCase("function: end;")]
    [TestCase("function(): end;")]
    [TestCase("function(x): end;")]
    [TestCase("function(x,y): end;")]
    [TestCase("function: function: function: end; end; end;")]
    [TestCase("function bar(i, j, k): end; bar(1, function(x): return function(x): return function(x): return function(x): return function:end end end end end, 1);")]
    public void VerifyFunctionInitializer(string input)
    {
        Assert.DoesNotThrow(() => Runtime!.Run(input));
    }

    [TestCase("x = function: return 0; end; result = x();", 0)]
    [TestCase("function x(func): return func(); end; result = x(function: return 0; end);", 0)]
    public void VerifyFirstClassFunction(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }

    [TestCase("function x(): return 0; end; result = x();", 0)]
    [TestCase("function x(y): return y; end; result = x(0);", 0)]
    [TestCase("function x(y): return y; end; z = 0; result = x(z);", 0)]
    [TestCase("function x(a, b): return a + b; end; a = 1; b = 1; result = x(a,b);", 2)]
    public void VerifyFunctionInvocation(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }

    [TestCase("function bar: end; x=bar(); assert(x==null);")]
    public void TestNullVoidReturn(string input)
    {
        Assert.DoesNotThrow(() => Runtime!.Run(input));
    }

    [TestCase("x = 1; function foo(i): x = 1; return x + i; end; result = foo(x);", 2)]
    public void VerifyPureFunctions(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }
}
