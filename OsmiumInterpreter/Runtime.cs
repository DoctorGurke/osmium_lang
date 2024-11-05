using Antlr4.Runtime;
using Osmium.Language;
using Osmium.Language.Types;

namespace Osmium;

public class RuntimeException : Exception
{
    public RuntimeException() : base() { }
    public RuntimeException(string message) : base(message) { }
}

public class LexerException : RuntimeException
{
    public LexerException() : base() { }
    public LexerException(string message) : base(message) { }
}

public class ParserException : RuntimeException
{
    public ParserException() : base() { }
    public ParserException(string message) : base(message) { }
}

public class ReturnException : Exception
{
    public object? Value { get; private set; }

    public ReturnException(object? value)
    {
        Value = value;
    }
}

public class AssertException : Exception
{
    public AssertException(string message) : base(message) { }
}

/// <summary>
/// Interpreter interface to run osmium programs. 
/// Maintains a symbol table but supports running programs independently as local.
/// </summary>
public class Runtime : IMembers
{
    public SymbolTable Members => SymbolTable;
    private SymbolTable SymbolTable { get; set; }
    public bool Debug { get; private set; }
    public static Runtime? Instance { get; set; }

    public Runtime(bool debug = false)
    {
        Instance = null;
        Instance = this;

        SymbolTable = new SymbolTable();
        SymbolTable.UpdateIntrinsicFunctions();
        Debug = debug;
    }

    private Dictionary<string, IFunction>? _intrinsicFunctions { get; set; }
    public Dictionary<string, IFunction> GetIntrinsicFunctions()
    {
        if (_intrinsicFunctions is not null)
            return _intrinsicFunctions;

        var intrinsics = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
        .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
        .Where(x => x.IsClass) // only yields classes
        .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
        .Where(x => x.IsStatic && x.GetCustomAttributes(typeof(IntrinsicFunctionAttribute), false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute

        var dict = new Dictionary<string, IFunction>();

        foreach (var symbol in intrinsics)
        {
            var attribute = symbol.GetCustomAttributes(typeof(IntrinsicFunctionAttribute), false).FirstOrDefault();
            if (attribute is not IntrinsicFunctionAttribute intrinsic)
                continue;

            var identifier = intrinsic.Identifier;
            var parameters = intrinsic.Parameters;
            var method = new MethodWrapper(identifier, symbol, parameters);
            dict.Add(identifier, method);
        }

        _intrinsicFunctions = dict;
        return dict;
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
        var visitor = new Interpreter() { Debug = this.Debug };
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
    /// <exception cref="LexerException"></exception>
    /// <exception cref="ParserException"></exception>
    private static OsmiumParser.FileContext ParseProgram(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var lexerListener = new ErrorListener<int>();
        var tokenStream = new CommonTokenStream(lexer);

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(lexerListener);

        if (lexerListener.HadError)
            throw new LexerException($"Lexer failed {lexerListener.GetErrorMessage()}");

        // try lexing token stream from input program
        tokenStream.Fill();

        var parser = new OsmiumParser(tokenStream);
        var parserListener = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(parserListener);

        // try parse token stream as file context
        var tree = parser.file();

        if (parserListener.HadError)
            throw new ParserException($"Parser failed: {parserListener.GetErrorMessage()}");

        return tree;
    }
}
