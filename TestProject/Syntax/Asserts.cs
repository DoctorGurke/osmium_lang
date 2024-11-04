namespace TestProject.Syntax;

[TestFixture]
public class Asserts : OsmiumTestRunner
{
    [TestCase("assert(true);")]
    [TestCase("x=1;assert(x==1);")]
    [TestCase("x=true;y=false;assert(x!=y);")]
    [TestCase("x=1;y=10;assert(x+y==11);")]
    public void TestAssert(string input)
    {
        Assert.DoesNotThrow(() => Runtime!.Run(input));
    }

    [TestCase("assert(false);")]
    [TestCase("assert(true==false);")]
    [TestCase("assert(1==2);")]
    [TestCase("assert(1+1==3);")]
    public void TestAssertFailure(string input)
    {
        Assert.Throws<AssertException>(() => Runtime!.Run(input));
    }
}
