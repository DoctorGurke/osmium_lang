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
        //Log.Info($">\t{input.Replace("\n", ">\t")}\n<EOF>");
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

        Log.Space();

        var visitor = new Interpreter();
        visitor.SymbolTable = SymbolTable;
        var target = visitor.Visit(tree);
        Log.Info($"{target}");
    }

    public void RunLocal(string program)
    {

    }
}
