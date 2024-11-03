using Antlr4.Runtime;

namespace Osmium.Interpreter;

public class RuntimeException : Exception
{
    public RuntimeException() : base() { }
    public RuntimeException(string message) : base(message) { }
}

/// <summary>
/// Interpreter interface to run osmium programs. 
/// Maintains a symbol table but supports running programs independently as local.
/// </summary>
public class Runtime : IMembers
{
    public SymbolTable Members => SymbolTable;
    private SymbolTable SymbolTable { get; set; }


    public Runtime()
    {
        SymbolTable = new SymbolTable();
    }

    public void Run(string input, bool local = false)
    {
        var program = ParseProgram(input);

        // attach runtime symbol table for direct evaluation
        var visitor = new Interpreter();
        if (!local)
            visitor.OverrideSymbolTable(SymbolTable);

        var programReturn = visitor.Visit(program);
        if (programReturn != null)
            Log.Info($"{programReturn}");
    }

    /// <summary>
    /// Run lexer and parser to get FileContext of given input program.
    /// </summary>
    /// <param name="input">Unvalidated Osmium program input.</param>
    /// <returns>Parsed file context on success.</returns>
    private OsmiumParser.FileContext ParseProgram(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.HadError;
        if (error)
            throw new RuntimeException($"Lexer failed {listener_lexer.GetErrorMessage()}");

        // get parsed token stream
        tokenStream.Fill();

        //-> tokenStream;

        var parser = new OsmiumParser(tokenStream);
        var listener_parser = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(listener_parser);

        // start parsing token stream to tree
        var tree = parser.file();

        error = listener_parser.HadError;
        if (error)
            throw new RuntimeException($"Parser failed: {listener_parser.GetErrorMessage()}");

        return tree;
    }
}
