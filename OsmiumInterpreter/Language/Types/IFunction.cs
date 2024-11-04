namespace Osmium.Language.Types;

public interface IFunction
{
    public object? Invoke(Interpreter visitor, object[] parameters);
}
