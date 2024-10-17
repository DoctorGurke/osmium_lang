namespace Osmium.Interpreter.Operators;

public static class Arithmetic
{
    // try operation as (op operand)
    public static object TryUnary(int op, object operand)
    {
        if (operand is bool @bool)
        {
            return TryUnaryBooleanArithmetic(op, @bool);
        }

        throw new InvalidOperationException($"Invalid unary operation: {op} {operand?.GetType()}");
    }

    public static object TryUnaryBooleanArithmetic(int op, bool obj)
    {
        switch (op)
        {
            case OsmiumParser.OP_LOGICAL_NOT: return !obj;
        }

        throw new InvalidOperationException($"Invalid bool operation: {op} {obj}");
    }

    // try operation as (operand op operand)
    public static object TryBinary(int op, object operand1, object operand2)
    {
        switch (op)
        {
            case OsmiumParser.OP_EQUALITY: return operand1.Equals(operand2);
            case OsmiumParser.OP_INEQUALITY: return !operand1.Equals(operand2);
        }

        if (operand1 is int @int)
        {
            return TryBinaryIntegerArithmetic(op, @int, operand2);
        }
        else if (operand1 is float @float)
        {
            return TryBinaryFloatArithmetic(op, @float, operand2);
        }
        else if (operand1 is bool @bool)
        {
            return TryBinaryBooleanArithmetic(op, @bool, operand2);
        }

        throw new InvalidOperationException($"Invalid binary operation: {op} [{operand1?.GetType()}]->[{operand2?.GetType()}]");
    }

    // check for binary operation against int
    private static object TryBinaryIntegerArithmetic(int op, int obj, object operand)
    {
        if (operand is int @int)
        {
            switch (op)
            {
                case OsmiumParser.OP_ADDITION: return obj + @int;
                case OsmiumParser.OP_SUBTRACTION: return obj - @int;
                case OsmiumParser.OP_MULTIPLY: return obj * @int;
                case OsmiumParser.OP_DIVISION: return obj / @int;

                case OsmiumParser.OP_MODULUS: return obj % @int;

                case OsmiumParser.OP_LESS_THAN: return obj < @int;
                case OsmiumParser.OP_GREATER_THAN: return obj > @int;
                case OsmiumParser.OP_LESS_THAN_OR_EQUALS: return obj <= @int;
                case OsmiumParser.OP_GREATER_THAN_OR_EQUALS: return obj >= @int;
            }
        }
        else if (operand is float @float)
        {
            switch (op)
            {
                case OsmiumParser.OP_ADDITION: return obj + @float;
                case OsmiumParser.OP_SUBTRACTION: return obj - @float;
                case OsmiumParser.OP_MULTIPLY: return obj * @float;
                case OsmiumParser.OP_DIVISION: return obj / @float;

                case OsmiumParser.OP_LESS_THAN: return obj < @float;
                case OsmiumParser.OP_GREATER_THAN: return obj > @float;
                case OsmiumParser.OP_LESS_THAN_OR_EQUALS: return obj <= @float;
                case OsmiumParser.OP_GREATER_THAN_OR_EQUALS: return obj >= @float;
            }
        }

        throw new InvalidOperationException($"Invalid int operation: op:{op} obj:{obj} operand:{operand}|{operand?.GetType()}");
    }

    // check for binary operation against float
    private static object TryBinaryFloatArithmetic(int op, float obj, object operand)
    {
        if (operand is int @int)
        {
            switch (op)
            {
                case OsmiumParser.OP_ADDITION: return obj + @int;
                case OsmiumParser.OP_SUBTRACTION: return obj - @int;
                case OsmiumParser.OP_MULTIPLY: return obj * @int;
                case OsmiumParser.OP_DIVISION: return obj / @int;

                case OsmiumParser.OP_LESS_THAN: return obj < @int;
                case OsmiumParser.OP_GREATER_THAN: return obj > @int;
                case OsmiumParser.OP_LESS_THAN_OR_EQUALS: return obj <= @int;
                case OsmiumParser.OP_GREATER_THAN_OR_EQUALS: return obj >= @int;
            }
        }
        else if (operand is float @float)
        {
            switch (op)
            {
                case OsmiumParser.OP_ADDITION: return obj + @float;
                case OsmiumParser.OP_SUBTRACTION: return obj - @float;
                case OsmiumParser.OP_MULTIPLY: return obj * @float;
                case OsmiumParser.OP_DIVISION: return obj / @float;

                case OsmiumParser.OP_MODULUS: return obj % @float;

                case OsmiumParser.OP_LESS_THAN: return obj < @float;
                case OsmiumParser.OP_GREATER_THAN: return obj > @float;
                case OsmiumParser.OP_LESS_THAN_OR_EQUALS: return obj <= @float;
                case OsmiumParser.OP_GREATER_THAN_OR_EQUALS: return obj >= @float;
            }
        }

        throw new InvalidOperationException($"Invalid float operation: {op} {obj} {operand?.GetType()}");
    }

    private static object TryBinaryBooleanArithmetic(int op, bool obj, object operand)
    {
        if (operand is bool @bool)
        {
            switch (op)
            {
                case OsmiumParser.OP_LOGICAL_AND: return obj && @bool;
                case OsmiumParser.OP_LOGICAL_OR: return obj || @bool;
            }
        }
        throw new InvalidOperationException($"Invalid bool operation: {op} {obj} {operand?.GetType()}");
    }
}
