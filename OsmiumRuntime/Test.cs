﻿using NUnit.Framework;

namespace OsmiumRuntime;

[TestFixture]
public class Test
{
    private ConsoleRunner? runner;

    [SetUp]
    public void Setup()
    {
        var runtime = new Osmium.Runtime();
        runner = new ConsoleRunner(runtime);
    }

    [Test]
    public void TestInitialization()
    {
        // given a runner and basic expression var name and value
        if (runner is null)
            return;
        string var = "x";
        int val = 5;

        // when evaluating the expression
        runner.ProcessInput($"{var}={val}");

        // then the symbol and value should match
        int value = (int)runner.Members.GetSymbolValue(var);
        Assert.That(value, Is.EqualTo(val));
    }

    [Test]
    public void TestFibonacci()
    {
        // given a runner
        if (runner is null)
            return;

        // when including a script and using a function from it
        runner.ProcessInput("include examples/fibonacci.script");
        runner.ProcessInput("x = fibonacci(10);");

        // then the return value should match the expected function
        Assert.That(runner.Members.GetSymbolValue("x"), Is.EqualTo(55));
    }

    [Test]
    public void TestLocalRun()
    {
        // given a runner and declared symbol
        if (runner is null)
            return;

        // when declaring the same symbol twice via local run
        runner.ProcessInput("run examples/fibonacci.script");

        // then allow definition in global scope
        runner.ProcessInput("fibonacci=1;");
        Assert.That(runner.Members.GetSymbolValue("fibonacci"), Is.EqualTo(1));
    }
}
