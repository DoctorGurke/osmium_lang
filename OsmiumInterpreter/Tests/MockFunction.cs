using Osmium.Language;
using Osmium.Language.Types;

namespace Osmium.Tests;

internal sealed class MockFunction : IFunction
{
    public object? Invoke(Interpreter visitor, object[] parameters)
    {
        throw new NotImplementedException();
    }
}
