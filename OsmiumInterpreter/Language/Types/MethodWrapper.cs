using System.Reflection;
using System.Runtime.ExceptionServices;

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
        try
        {
            return Method.Invoke(null, new object[] { parameters });
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException!).Throw();
        }

        return null;
    }

    public override string ToString()
    {
        return $"Function:[{Identifier}] Params:[{string.Join(", ", Parameters)}]";
    }
}
