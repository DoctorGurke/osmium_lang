using Osmium.Language.Types;

namespace TestProject.Internal;

internal sealed class MockFunction : IFunction
{
    public object? Invoke(Interpreter visitor, object[] parameters)
    {
        throw new NotImplementedException();
    }
}
