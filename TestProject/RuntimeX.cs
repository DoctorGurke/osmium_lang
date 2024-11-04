namespace TestProject;

public static class RuntimeX
{
    public static void VerifyResult(this Runtime runtime, string input, object result, string symbolName = "result")
    {
        Assert.DoesNotThrow(() => { runtime!.Run(input); });
        var output = runtime!.Members.GetSymbolValue(symbolName);
        Assert.That(output, Is.EqualTo(result));
    }
}
