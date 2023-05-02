using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Osmium.Interpreter;

public class ParseTreeListener : OsmiumParserBaseListener
{
    public override void EnterEveryRule([NotNull] ParserRuleContext context)
    {
        Log.Info($"[{context.Depth()}]\t{OsmiumParser.ruleNames[context.RuleIndex]}");
    }
}
