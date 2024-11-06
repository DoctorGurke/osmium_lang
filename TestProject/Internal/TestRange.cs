using Range = Osmium.Language.Types.Range;

namespace TestProject.Internal;

[TestFixture]
public class TestRange
{
    [SetUp]
    public void SetUp()
    {

    }

    [TestCase(null, 1)]
    [TestCase(1, null)]
    [TestCase(0, 1)]
    public void VerifyConstructor(int? start, int? end)
    {
        Assert.DoesNotThrow(() => { new Range(start, end); });
    }

    [TestCase(null, null)]
    [TestCase(3, 1)]
    public void VerifyInvalidConstructor(int? start, int? end)
    {
        Assert.Throws<ArgumentException>(() => { new Range(start, end); });
    }
}
