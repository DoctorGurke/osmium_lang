using Antlr4.Runtime;

namespace Osmium.Interpreter;

public class Runtime
{
    public SymbolTable SymbolTable { get; set; }

    public Runtime()
    {
        SymbolTable = new SymbolTable();
    }

    public void Run(string input)
    {
        var program = Evaluate(input);
        InterpretTree(program);
    }

    public void RunLocal(string input)
    {
        var program = Evaluate(input);
        InterpretTree(program, true);
    }

    private void InterpretTree(OsmiumParser.FileContext file, bool local = false)
    {
        var visitor = new Interpreter();

        // attach runtime symbol table for non-local evaluation (direct command line input)
        if (!local)
            visitor.SymbolTable = SymbolTable;

        var target = visitor.Visit(file);
        if (target != null)
            Log.Info($"{target}"); // program exit
    }

    /// <summary>
    /// Run lexer and parser to get FileContext of given input program.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private OsmiumParser.FileContext Evaluate(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.had_error;
        if (error)
            Log.Info($"Lexer failed!");

        // get parsed token stream
        tokenStream.Fill();

        //-> tokenStream;

        var parser = new OsmiumParser(tokenStream);
        var listener_parser = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(listener_parser);

        // start parsing token stream to tree
        var tree = parser.file();

        error = listener_parser.had_error;
        if (error)
            Log.Info($"Parser failed!");

        return tree;
    }
}
