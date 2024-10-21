using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using System.Text;

namespace Osmium.Interpreter;

[TestFixture]
public class Test
{
    [Test]
    public void Main()
    {
        //DumpTokens(GetScript("test/lexer/functions.script"));
        TryScriptFile("test.script");
    }

    [TestCase("test/syntax/syntax_error.script")]
    public void TestSyntaxError(string script)
    {
        TryScript(Script.Load(script), verifyFailure: true);
    }

    [TestCase("examples/scopes.script")]
    [TestCase("examples/fibonacci.script")]
    [TestCase("examples/control_flow.script")]
    [TestCase("examples/pure_functions.script")]
    [TestCase("examples/functions.script")]
    [TestCase("examples/list.script")]
    [TestCase("examples/arithmetic.script")]
    [TestCase("examples/map.script")]
    [TestCase("examples/reduce.script")]
    [TestCase("examples/filter.script")]
    [TestCase("examples/foreach.script")]
    [TestCase("examples/print.script")]
    [TestCase("examples/range_index_recursion.script")]
    [TestCase("base_functions.script")]
    [TestCase("test.script")]
    [TestCase("test/lexer/control_flow.script")]
    [TestCase("test/lexer/assignment.script")]
    [TestCase("test/lexer/expressions.script")]
    [TestCase("test/lexer/booleans.script")]
    [TestCase("test/lexer/functions.script")]
    [TestCase("test/lexer/assert.script")]
    [TestCase("test/lexer/enums.script")]
    [TestCase("test/lexer/namespaces.script")]
    public void TestScriptFile(string script)
    {
        TryScriptFile(script);
    }

    [TestCase("test/lexer/functions.script", new string[]
    {
        "FUNCTION",
        "VARIABLE",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "OP_ASSIGN",
        "FUNCTION",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "OP_ASSIGN",
        "FUNCTION",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "OP_ASSIGN",
        "FUNCTION",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "COLON",
        "FUNCTION",
        "COLON",
        "FUNCTION",
        "COLON",
        "END",
        "SEMICOLON",
        "END",
        "SEMICOLON",
        "END",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "FUNCTION",
        "COLON",
        "END",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "STRING",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "FUNCTION",
        "COLON",
        "END",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "INT",
        "COMMA",
        "FUNCTION",
        "COLON",
        "END",
        "COMMA",
        "STRING",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "VARIABLE",
        "OP_ASSIGN",
        "LEFT_BRACKET",
        "LAMBDA",
        "COLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "STRING",
        "RIGHT_BRACKET",
        "END",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "FUNCTION",
        "COLON",
        "END",
        "END",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "FUNCTION",
        "VARIABLE",
        "LEFT_BRACKET",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "COMMA",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "INT",
        "COMMA",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "FUNCTION",
        "COLON",
        "END",
        "END",
        "COMMA",
        "INT",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "VARIABLE",
        "LEFT_BRACKET",
        "INT",
        "COMMA",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "LAMBDA",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "FUNCTION",
        "COLON",
        "END",
        "END",
        "END",
        "END",
        "END",
        "COMMA",
        "INT",
        "RIGHT_BRACKET",
        "SEMICOLON",
        "EOF"
    })]
    /*[TestCase("test/lexer/tokens_all.script", new string[] {
            "LEFT_BRACKET",
            "RIGHT_BRACKET",
            "LEFT_SQUARE_BRACKET",
            "RIGHT_SQUARE_BRACKET",
            "POINT",
            "COMMA",
            "APOSTROPHE",
            "QUOTE",
            "SEMICOLON",
            "COLON",
            "IF",
            "ELSE",
            "WHILE",
            "FUNCTION",
            "RETURN",
            "END",
            "BOOLEAN",
            "BOOLEAN",
            "NULL",
            "VARIABLE",
            "ASSIGNMENT",
            "ADD_ASSIGNMENT",
            "SUB_ASSIGNMENT",
            "MUL_ASSIGNMENT",
            "DIV_ASSIGNMENT",
            "INT",
            "INT",
            "FLOAT",
            "FLOAT",
            "FLOAT",
            "FLOAT",
            "DOUBLE",
            "DOUBLE",
            "CHAR",
            "STRING",
            "NOT",
            "NOT",
            "AND",
            "AND",
            "OR",
            "OR",
            "EQUALS",
            "GREATER",
            "LESS",
            "GREATER_EQUALS",
            "LESS_EQUALS",
            "OP_POW",
            "OP_MUL",
            "OP_DIV",
            "OP_MOD",
            "OP_ADD",
            "OP_SUB"
        })]*/
    public void TestTokenStreamFromScript(string script, params string[] testTokens)
    {
        var input = Script.Load(script);

        TestTokenStream(input, testTokens);
    }

    public static void TestTokenStream(string input, params string[] testTokens)
    {
        var str = new AntlrInputStream(input);
        var lexer = new OsmiumLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.HadError;
        Assert.That(actual: error, expression: Is.False, message: $"{listener_lexer.GetErrorMessage()}");

        Log.Info($"Lexer {(error ? "Failed" : "Passed")}.");

        // get parsed token stream
        tokenStream.Fill();
        var tokens = tokenStream.GetTokens();

        Log.Info($"[{input.Length} chars] -> [{tokens.Count} tokens]\n");
        foreach (var token in tokens)
        {
            var tokenName = lexer.Vocabulary.GetSymbolicName(token.Type);
            Log.Info($"[{token.Line}|{token.Column}] {tokenName}({token.Type}) \t : {token.Text}");
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            if (i >= testTokens.Length)
                break;

            var testToken = testTokens[i];
            var token = tokens[i];

            var tokenName = lexer.Vocabulary.GetSymbolicName(token.Type);
            Log.Info($"[{testToken}] -> [{tokenName}]");
            var testTokenType = lexer.GetTokenType(testToken);

            Assert.That(token.Type == testTokenType, Is.True);
        }
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
        DumpTokens(tokenStream, lexer);
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

        Log.Info($"Syntax Tree:\n{tree.ToStringTree(parser)}");

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

    // used for building tests
    private static string DumpTokens(CommonTokenStream tokenStream, OsmiumLexer lexer)
    {
        var tokens = tokenStream.GetTokens();

        var sb = new StringBuilder();
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            sb.Append($"{lexer.Vocabulary.GetSymbolicName(token.Type)} ");
        }

        var dump = sb.ToString().Trim();
        Log.Info(dump);
        return dump;
    }
}