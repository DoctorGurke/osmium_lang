using Enum = Osmium.Language.Types.Enum;

namespace TestProject.Internal;

[TestFixture]
public class TestEnum
{
    [SetUp]
    public void SetUp()
    {

    }

    [Test]
    public void VerifyMemberCount()
    {
        // given an empty enum definition
        var identifier = "test_enum";
        var members = new Dictionary<string, int?>() { };

        // then creating an enum object should throw due to 0 member count
        Assert.Throws<ArgumentException>(() => { _ = new Enum(identifier, members); });
    }

    [Test]
    public void VerifyUnsignedMembers()
    {
        // given an enum definition with negative values
        var identifier = "test_enum";
        var members = new Dictionary<string, int?>()
        {
            {"member1", null },
            {"member2", 1 },
            {"member3", -1 },
        };

        // then creating an enum object should throw due to signed member values
        Assert.Throws<ArgumentException>(() => { _ = new Enum(identifier, members); });
    }

    [Test]
    public void VerifyInvalidIndexOf()
    {
        // given an enum definition
        var identifier = "test_enum";
        var members = new Dictionary<string, int?>()
        {
            {"member1", null },
            {"member2", 5 },
            {"member3", null },
            {"member4", 17 },
        };
        var enumObj = new Enum(identifier, members);

        // then trying to get indexOf invalid member should throw
        Assert.Throws<Exception>(() => { enumObj.GetIndexOf(20); });
    }

    [Test]
    public void VerifyIndexOf()
    {
        // given an enum definition
        var identifier = "test_enum";
        var members = new Dictionary<string, int?>()
        {
            {"member1", null },
            {"member2", 5 },
            {"member3", null },
            {"member4", 17 },
        };
        var enumObj = new Enum(identifier, members);

        // when getting indexOf enum member value
        var member2 = enumObj.GetIndexOf(5);

        // then member name should match
        Assert.That(member2, Is.EqualTo("member2"));
    }
}
