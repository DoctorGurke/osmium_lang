namespace Osmium.Language;

using Antlr4.Runtime;
using System.IO;

public class ErrorListener<S> : ConsoleErrorListener<S>
{
    public bool HadError;
    private string? _message;

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
        int col, string msg, RecognitionException e)
    {
        HadError = true;
        _message = $"{msg} line {line}:{col}";
        base.SyntaxError(output, recognizer, offendingSymbol, line, col, $"Syntax Error: {msg}", e);
    }

    public string GetErrorMessage()
    {
        if (!HadError || _message is null)
            return "";

        return _message;
    }
}
