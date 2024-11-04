namespace TestProject.Syntax;

[TestFixture]
public class ControlFlow : OsmiumTestRunner
{
    [TestCase("function x(y): if(y): return 1; else: return 0; end; end; result = x(true);", 1)]
    [TestCase("function x(y): if(y): return 1; else: return 0; end; end; result = x(false);", 0)]
    [TestCase("function foo(x): if(x == 0): return 0; else if(x == 1): return 1; else: return -1; end; end; result = foo(1);", 1)]
    public void TestIfElseFlow(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }
}
