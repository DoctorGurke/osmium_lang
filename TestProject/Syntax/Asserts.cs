namespace TestProject.Syntax;

[TestFixture]
public class Asserts
{
    private Runtime? runtime;

    [SetUp]
    public void SetUp()
    {
        runtime = null;
        runtime = new Runtime();
    }

    [TestCase("assert(true);")]
    [TestCase("x=1;assert(x==1);")]
    [TestCase("x=true;y=false;assert(x!=y);")]
    [TestCase("x=1;y=10;assert(x+y==11);")]
    public void TestAssert(string input)
    {
        Assert.DoesNotThrow(() => runtime!.Run(input));
    }

    [TestCase("assert(false);")]
    [TestCase("assert(true==false);")]
    [TestCase("assert(1==2);")]
    [TestCase("assert(1+1==3);")]
    public void TestAssertFailure(string input)
    {
        Assert.Throws<AssertException>(() => runtime!.Run(input));
    }
}
