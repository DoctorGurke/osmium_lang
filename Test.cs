using Antlr4.Runtime;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Text;

namespace OsmiumLang;

[TestFixture]
public class Test
{
    [Test]
    public void Main()
    {
        //DumpTokens(GetScript("test/lexer/functions.script"));
        TryScript("test.script");
    }

    [TestCase("test/syntax/syntax_error.script")]
    public void TestSyntaxError(string script)
    {
        TryError(script);
    }

    [TestCase("base_functions.script")]
    [TestCase("test/lexer/control_flow.script")]
    [TestCase("test/lexer/assignment.script")]
    [TestCase("test/lexer/expressions.script")]
    [TestCase("test/lexer/booleans.script")]
    [TestCase("test/lexer/functions.script")]
    public void TestScriptFile(string script)
    {
        TryScript(script);
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
        "ASSIGNMENT",
        "FUNCTION",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "ASSIGNMENT",
        "FUNCTION",
        "LEFT_BRACKET",
        "VARIABLE",
        "RIGHT_BRACKET",
        "COLON",
        "END",
        "SEMICOLON",
        "VARIABLE",
        "ASSIGNMENT",
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
        "VARIABLE",
        "LEFT_BRACKET",
        "FUNCTION",
        "COLON",
        "END",
        "RIGHT_BRACKET",
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
        "ASSIGNMENT",
        "LEFT_BRACKET",
        "LAMBDA",
        "COLON",
        "END",
        "RIGHT_BRACKET",
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
    [TestCase("test/lexer/tokens_all.script", new string[] {
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
        })]
    public void TestTokenStreamFromScript(string script, params string[] testTokens)
    {
        var input = GetScript(script);

        TestTokenStream(input, testTokens);
    }

    public static void TestTokenStream(string input, params string[] testTokens)
    {
        var str = new AntlrInputStream(input);
        var lexer = new ArithmeticLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.had_error;
        Assert.False(error);

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

            Assert.True(token.Type == testTokenType);
        }
    }

    public static CommonTokenStream RunLexer(string input, bool verifyError = false)
    {
        Log.Info($">\t{input.Replace("\n", ">\t")}\n<EOF>");
        var str = new AntlrInputStream(input);
        var lexer = new ArithmeticLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        var error = listener_lexer.had_error;
        Log.Info($"\nLexer {(error ? "Failed" : "Passed")}.\n ");
        // verify that input is erroneous 
        if (verifyError && error)
            Assert.Pass();

        Assert.False(error);


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
        var parser = new ArithmeticParser(tokens);
        var listener_parser = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(listener_parser);

        // parse token stream
        var tree = parser.file();

        var error = listener_parser.had_error;
        Log.Info($"Parser {(error ? "Failed" : "Passed")}.");
        // verify that input is erroneous 
        if (verifyError && error)
            Assert.Pass();

        Log.Info($"Syntax Tree:\n{tree.ToStringTree(parser)}");
        //Log.Info($"Info:\n{tree.ToInfoString(parser)}");
        Log.Info("\n");

        Assert.False(error);
        return true;
    }

    public static void Try(string input)
    {
        var tokens = RunLexer(input);
        RunParser(tokens);
    }

    public static void TryError(string script)
    {
        var tokens = RunLexer(GetScript(script), true);
        RunParser(tokens, true);

        Assert.Fail();
    }

    public static void TryScript(string script)
    {
        Try(GetScript(script));
    }

    private static string GetScript(string script)
    {
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var scriptsPath = Path.Combine(executableLocation, "scripts");

        var fileFullPath = Path.IsPathFullyQualified(script) ? script : Path.Combine(scriptsPath, script);

        try
        {
            var fileContent = File.ReadAllText(fileFullPath);
            return fileContent;
        }
        catch
        {
            Log.Info($"script not found: {fileFullPath}");

            throw new ScriptNotFoundException(script);
        }
    }

    // used for building tests
    private static string DumpTokens(CommonTokenStream tokenStream, ArithmeticLexer lexer)
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