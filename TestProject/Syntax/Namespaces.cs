namespace TestProject.Syntax;

[TestFixture]
public class Namespaces : OsmiumTestRunner
{
    [TestCase("scope{x=1;}; result=scope.x;", 1)]
    [TestCase("scope{x=1f;}; result=scope.x;", 1f)]
    [TestCase("scope{x=\"a\";}; result=scope.x;", "a")]
    [TestCase("scope{function x():return 1 end;}; result=scope.x();", 1)]
    [TestCase("scope{enum x=[a,b,c];}; result=scope.x.b;", 1)]
    [TestCase("scope1{scope2{x=1;};}; result=scope1.scope2.x;", 1)]
    public void TestNamespaceMembers(string input, object value)
    {
        Runtime!.VerifyResult(input, value);
    }
}
