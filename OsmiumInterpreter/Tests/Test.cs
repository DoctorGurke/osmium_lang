using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;

namespace Osmium.Language;

[TestFixture]
public class Test
{
    [Test]
    public void Main()
    {
        //DumpTokens(GetScript("test/lexer/functions.script"));
        TryScriptFile("test.script");
    }

    public void SyntaxError(string script)
    {
        TryScript(Script.Load(script), verifyFailure: true);
    }

    [TestCase("examples/control_flow.script")]
    [TestCase("examples/pure_functions.script")]
    [TestCase("examples/functions.script")]
    [TestCase("examples/map.script")]
    [TestCase("examples/reduce.script")]
    [TestCase("examples/filter.script")]
    [TestCase("examples/foreach.script")]
    [TestCase("examples/print.script")]
    [TestCase("examples/range_index_recursion.script")]
    [TestCase("base_functions.script")]
    [TestCase("test/lexer/control_flow.script")]
    [TestCase("test/lexer/functions.script")]
    [TestCase("test/lexer/assert.script")]
    [TestCase("test/lexer/operator_priority.script")]
    public void ScriptFile(string script)
    {
        TryScriptFile(script);
    }

    public static CommonTokenStream RunLexer(string input, bool verifyError = false)
    {
        Log.Info($">\t{input.Replace("\n", ">\t")}\n<EOF>");
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.HadError;
        Log.Info($"\nLexer {(error ? "Failed" : "Passed")}.\n ");
        // verify that input is erroneous 
        if (verifyError && error)
            Assert.Pass();

        Assert.That(error, Is.False);


        // get parsed token stream
        tokenStream.Fill();
        var tokens = tokenStream.GetTokens();

        Log.Info($"TOKENS:");
        Log.Info($"[{input.Length} chars] -> [{tokens.Count} tokens]");
        Log.Info("-----");

        foreach (var token in tokens)
        {
            var tokenName = lexer.Vocabulary.GetSymbolicName(token.Type);
            Log.Info($"[{token.Line}|{token.Column}] {tokenName} \t : {token.Text}");
        }
        return tokenStream;
    }

    public static bool RunParser(CommonTokenStream tokens, bool verifyError = false)
    {
        var parser = new OsmiumParser(tokens);
        var listener_parser = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(listener_parser);

        // start parsing token stream to tree
        var tree = parser.file();
        var listener = new ParseTreeListener();

        var error = listener_parser.HadError;
        Log.Info($"Parser {(error ? "Failed" : "Passed")}.");
        // verify that input is erroneous 
        if (verifyError && error)
            Assert.Pass();

        Log.Space();

        Log.Info($"---Walk Tree---");
        var walker = new ParseTreeWalker();
        walker.Walk(listener, tree);

        Log.Space();

        Log.Info($"---Visit Tree---");
        var visitor = new Interpreter();
        Interpreter.Debug = true;
        var target = visitor.Visit(tree);
        Interpreter.Debug = false;

        Log.Info($"exit {target}");

        Assert.That(actual: error, expression: Is.False, message: $"{listener_parser.GetErrorMessage()}");
        return true;
    }

    public static void TryScript(string input, bool verifyFailure = false)
    {
        var tokens = RunLexer(input, verifyError: verifyFailure);
        RunParser(tokens, verifyError: verifyFailure);

        // parser should pass assert on error
        if (verifyFailure)
            Assert.Fail();
    }

    public static void TryScriptFile(string script)
    {
        TryScript(Script.Load(script));
    }
}