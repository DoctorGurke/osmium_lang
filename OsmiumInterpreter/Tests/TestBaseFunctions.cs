using NUnit.Framework;
using Osmium.Language;

namespace Osmium.Tests;

[TestFixture]
public class TestBaseFunctions
{
    [SetUp]
    public void SetUp()
    {

    }

    [TestCase("Hello, World!")]
    [TestCase("")]
    [TestCase(1)]
    [TestCase(-0.1f)]
    [TestCase(null)]
    public void VerifyPrint(params object[] parameters)
    {
        Assert.DoesNotThrow(() => Intrinsics.Print(parameters));
    }

    [Test]
    public void VerifyListPrint()
    {
        var list = new int[] { 1, 2, 3 };
        Assert.DoesNotThrow(() => Intrinsics.Print(new object[] { list.ToList() }));
    }

    [TestCase("", "")]
    [TestCase()]
    public void VerifyInvalidPrint(params object[] parameters)
    {
        Assert.Throws<ArgumentException>(() => Intrinsics.Print(parameters));
    }
}
