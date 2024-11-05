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

    [TestCase("", "")]
    [TestCase()]
    public void VerifyInvalidPrint(params object[] parameters)
    {
        Assert.Throws<ArgumentException>(() => Intrinsics.Print(parameters));
    }
}
