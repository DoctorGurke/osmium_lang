namespace TestProject;

public static class Test
{
    public static void VerifyResult(this Runtime runtime, string input, object result, string symbolName = "result")
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input); });
        var output = runtime!.Members.GetSymbolValue(symbolName);
        Assert.That(output, Is.EqualTo(result));
    }
}
