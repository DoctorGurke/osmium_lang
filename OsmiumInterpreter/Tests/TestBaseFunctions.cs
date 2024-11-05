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

    [TestCase(new object[] { }, 0)]
    [TestCase(new object[] { 1 }, 1)]
    [TestCase(new object[] { 1, 2, 3 }, 3)]
    public void VerifyLength(object[] list, int count)
    {
        int result = -1;
        Assert.DoesNotThrow(() => { result = Intrinsics.Length(new object[] { list.ToList() }); });
        Assert.That(result, Is.EqualTo(count));
    }

    [TestCase()]
    [TestCase("")]
    [TestCase(1, 2)]
    public void VerifyInvalidLength(params object[] parameters)
    {
        Assert.Throws<ArgumentException>(() => Intrinsics.Length(parameters));
    }

    [TestCase(new object[] { 1 })]
    [TestCase(new object[] { 1, 2, 3 })]
    public void VerifyForEach(object[] list)
    {
        Assert.Throws<NotImplementedException>(() => Intrinsics.ForEach(new object[] { list.ToList(), new MockFunction() }));
    }

    [TestCase()]
    [TestCase("")]
    [TestCase("", "", "")]
    public void VerifyInvalidForEach(params object[] parameters)
    {
        Assert.Throws<ArgumentException>(() => Intrinsics.ForEach(parameters));
    }
}
