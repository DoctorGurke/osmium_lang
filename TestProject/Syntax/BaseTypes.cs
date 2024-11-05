namespace TestProject.Syntax;

[TestFixture]
public class BaseTypes : OsmiumTestRunner
{
    [Test]
    public void VerifyNull()
    {
        Runtime!.VerifyResult("result = null;", null);
    }

    [TestCase("result = true;", true)]
    [TestCase("result = false;", false)]
    [TestCase("x = true; result = !x;", false)]
    [TestCase("result = true || false;", true)]
    [TestCase("result = true && false;", false)]
    public void VerifyBoolean(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("result = 1;", 1)]
    [TestCase("result = -1;", -1)]
    public void VerifyInt(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("result = 0.1f;", 0.1f)]
    [TestCase("result = -0.1f;", -0.1f)]
    [TestCase("result = 0.1;", 0.1f)]
    [TestCase("result = -0.1;", -0.1f)]
    [TestCase("result = .1f;", 0.1f)]
    [TestCase("result = -.1f;", -0.1f)]
    [TestCase("result = .1;", 0.1f)]
    [TestCase("result = -.1;", -0.1f)]
    [TestCase("result = 1f;", 1f)]
    [TestCase("result = -1f;", -1f)]
    public void VerifyFloat(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("result = \"Hello, World!\";", "Hello, World!")]
    [TestCase("result = \"äöüß'~^°\";", "äöüß'~^°")]
    public void VerifyString(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("x=\"foobar\";result=x[0];", "f")]
    [TestCase("x=\"foobar\";y=0;result=x[y];", "f")]
    [TestCase("x=\"foobar\";result=x[0..];", "foobar")]
    [TestCase("x=\"foobar\";result=x[..3];", "foob")]
    [TestCase("x=\"foobar\";result=x[1..3];", "oob")]
    public void VerifyStringIndexOf(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }

    [TestCase("result = [1,2,3];", new int[] { 1, 2, 3 })]
    [TestCase("result = [1f,2f,3f];", new float[] { 1f, 2f, 3f })]
    [TestCase("result = [\"a\",\"b\",\"c\"];", new string[] { "a", "b", "c" })]
    [TestCase("result = [];", new object[] { })]
    public void VerifyList(string input, object result)
    {
        if (result is object[] array)
            Runtime!.VerifyResult(input, array.ToList());
    }

    [TestCase("list = [1,2,3,4,5]; result = list[1..];", new int[] { 2, 3, 4, 5 })]
    [TestCase("list = [1,2,3,4,5]; result = list[..1];", new int[] { 1, 2 })]
    [TestCase("list = [1,2,3,4,5]; result = list[1..3];", new int[] { 2, 3 })]
    public void VerifyListRange(string input, object result)
    {
        if (result is object[] array)
            Runtime!.VerifyResult(input, array.ToList());
    }

    [TestCase("list = [1,2,3]; result = list[0];", 1)]
    [TestCase("list = [1,2,3]; x = 0; result = list[x];", 1)]
    [TestCase("list = [1,2,3]; x = 0..; result = list[x];", new int[] { 1, 2, 3 })]
    public void VerifyListIndexOf(string input, object result)
    {
        if (result is object[] array)
            Runtime!.VerifyResult(input, array.ToList());
        else
            Runtime!.VerifyResult(input, result);
    }

    [TestCase("enum x = [e1,e2,e3]; result = x.e1;", 0)]
    [TestCase("enum x = [e1,e2=2,e3]; result = x.e2;", 2)]
    [TestCase("enum x = [e1,e2=3,e3]; result = x.e3;", 1)]
    [TestCase("enum x = [e1,e2,e3]; result = x[0];", "e1")]
    [TestCase("enum x = [e1,e2=3,e3]; result = x[3];", "e2")]
    [TestCase("enum x = [e1,e2=3,e3]; y=3; result = x[y];", "e2")]
    public void VerifyEnum(string input, object result)
    {
        Runtime!.VerifyResult(input, result);
    }
}
