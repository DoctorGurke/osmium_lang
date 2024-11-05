using System.Reflection;

namespace Osmium.Language.Types;

public class MethodWrapper : IFunction
{
    public string Identifier { get; private set; }
    public MethodInfo Method { get; private set; }
    public string[] Parameters { get; private set; }

    public MethodWrapper(string identifier, MethodInfo method, string[] parameters)
    {
        Identifier = identifier;
        Method = method;
        Parameters = parameters;
    }

    public object? Invoke(Interpreter visitor, object[] parameters)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"Function:[{Identifier}] Params:[{string.Join(", ", Parameters)}]";
    }
}
