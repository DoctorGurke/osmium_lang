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
        //TryScript("test/syntax/syntax_error.script");
    }

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

        if (listener_lexer.had_error)
        {
            Assert.Fail();
        }

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

            if (token.Type != testTokenType)
                Assert.Fail();
        }

        Assert.Pass();
    }

    public static CommonTokenStream RunLexer(string input)
    {
        Log.Info($"> {input}");
        var str = new AntlrInputStream(input);
        var lexer = new ArithmeticLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        if (listener_lexer.had_error)
            Assert.Fail();
        else
            Log.Info("Lexer Passed.");

        tokenStream.Fill();

        var tokens = tokenStream.GetTokens();

        Log.Info($"[{input.Length} chars] -> [{tokens.Count} tokens]\n");
        foreach (var token in tokens)
        {
            var tokenName = lexer.Vocabulary.GetSymbolicName(token.Type);
            Log.Info($"[{token.Line}|{token.Column}] {tokenName} \t : {token.Text}");
        }
        return tokenStream;
    }

    public static void RunParser(CommonTokenStream tokens)
    {
        var parser = new ArithmeticParser(tokens);
        var listener_parser = new ErrorListener<IToken>();

        parser.RemoveErrorListeners();
        parser.AddErrorListener(listener_parser);

        var tree = parser.file();
        if (listener_parser.had_error)
            Assert.Fail();
        else
            Log.Info("Parser Passed.");

        Log.Info($"Syntax Tree:\n{tree.ToStringTree(parser)}");
        //Log.Info($"Info:\n{tree.ToInfoString(parser)}");
        Log.Info("\n");
    }

    public static void Try(string input)
    {
        var tokens = RunLexer(input);
        RunParser(tokens);
    }

    public static void TryScript(string script)
    {
        Try(GetScript(script));
    }

    private static string GetScript(string script)
    {
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var scriptsPath = Path.Combine(executableLocation, "scripts");
        var fileFullPath = Path.Combine(scriptsPath, script);

        try
        {
            var fileContent = File.ReadAllText(fileFullPath);
            return fileContent;
        }
        catch
        {
            Log.Info($"script not found: {fileFullPath}");
            return null;
        }
    }

    private static string DumpTokens(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new ArithmeticLexer(str);
        var tokenStream = new CommonTokenStream(lexer);
        var listener_lexer = new ErrorListener<int>();

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);

        if (listener_lexer.had_error)
            Log.Info("Lexer Failed.");
        else
            Log.Info("Lexer Passed.");

        tokenStream.Fill();

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