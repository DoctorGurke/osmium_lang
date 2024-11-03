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

    /// <summary>
    /// Run an osmium program.
    /// </summary>
    /// <param name="input">Osmium input program.</param>
    /// <param name="local"></param>
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
    /// <exception cref="RuntimeException"></exception>
    private static OsmiumParser.FileContext ParseProgram(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var lexerListener = new ErrorListener<int>();
        var tokenStream = new CommonTokenStream(lexer);

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(lexerListener);

        if (lexerListener.HadError)
            throw new RuntimeException($"Lexer failed {lexerListener.GetErrorMessage()}");

        // try lexing token stream from input program
        tokenStream.Fill();

        var parser = new OsmiumParser(tokenStream);
        var parserListener = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(parserListener);

        // try parse token stream as file context
        var tree = parser.file();

        if (parserListener.HadError)
            throw new RuntimeException($"Parser failed: {parserListener.GetErrorMessage()}");

        return tree;
    }
}
