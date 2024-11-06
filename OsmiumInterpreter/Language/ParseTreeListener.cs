using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Text;

namespace Osmium.Language;

public class ParseTreeListener : OsmiumParserBaseListener
{
    public override void EnterEveryRule([NotNull] ParserRuleContext context)
    {
        var pad = new StringBuilder();
        for (int i = 0; i < context.Depth() - 1; i++)
        {
            pad.Append("  ");
        }

        Log.Debug($"{pad}-{OsmiumParser.ruleNames[context.RuleIndex]}");
    }
}
