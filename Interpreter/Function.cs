namespace Osmium.Interpreter;

public class Function
{
    public OsmiumParser.Program_blockContext program { get; set; }
    public OsmiumParser.Identifier_listContext param_list { get; set; }
    public int ParameterCount { get; set; }

    public Function(OsmiumParser.Program_blockContext program, int parameterCount)
    {
        this.program = program;
        ParameterCount = parameterCount;
    }

    public object Invoke(object[] args)
    {
        if (args.Length != ParameterCount)
        {
            throw new ArgumentException($"Invalid parameter count.");
        }

        // set param symbols based on param_list identifiers and args objects

        // visit program

        return null;
    }
}
