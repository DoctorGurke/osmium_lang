using NUnit.Framework;
using System.Reflection;

namespace Osmium.Tests;

[TestFixture]
public class TestExamples
{
    public Runtime? Runtime { get; private set; }

    [SetUp]
    protected void SetUp()
    {
        Runtime = null;
        Runtime = new Runtime();
    }

    [TestCaseSource(nameof(ExampleScripts))]
    public void VerifyScript(string script)
    {
        Assert.DoesNotThrow(() => Runtime!.Run(Script.Load(script)));
    }

    public static IEnumerable<string> ExampleScripts()
    {
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception($"Couldn't find Executable location!");
        var examplesPath = Path.Combine(executableLocation, "scripts\\examples");

        var scripts = Directory.GetFiles(examplesPath, "*.script", searchOption: SearchOption.AllDirectories);
        foreach (var script in scripts)
        {
            yield return script.Replace($"{executableLocation}\\scripts\\", "");
        }
    }
}
