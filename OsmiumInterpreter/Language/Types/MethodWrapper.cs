using System.Reflection;

namespace Osmium.Language.Types;

public class MethodWrapper : IFunction
{
    public string Identifier { get; private set; }
    public MethodInfo Method { get; private set; }

    public MethodWrapper(string identifier, MethodInfo method)
    {
        Identifier = identifier;
        Method = method;
    }

    public object? Invoke(Interpreter visitor, object[] parameters)
    {
        throw new NotImplementedException();
    }
}
