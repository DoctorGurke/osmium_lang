using NUnit.Framework;

namespace Osmium;

[TestFixture]
public class Test
{
    [Test]
    public void TestInitialization()
    {
        var runtime = new Interpreter.Runtime();
        var instance = new ConsoleRunner(runtime);

        string var = "x";
        int val = 5;
        instance.ProcessInput($"{var}={val}");
        int value = (int)instance.Members.GetSymbolValue(var);
        Assert.That(value, Is.EqualTo(val));
    }
}
