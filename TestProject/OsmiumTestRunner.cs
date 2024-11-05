namespace TestProject;

public abstract class OsmiumTestRunner
{
    public Runtime? Runtime { get; private set; }

    [SetUp]
    protected void SetUp()
    {
        Runtime = null;
        Runtime = new Runtime(debug: true);

    }
}
