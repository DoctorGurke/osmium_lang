﻿namespace Osmium.Interpreter;

public interface IFunction
{
    public object? Invoke(Interpreter visitor, object[] parameters);
}
