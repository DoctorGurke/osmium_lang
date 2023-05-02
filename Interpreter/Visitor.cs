using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace Osmium.Interpreter;

public class Visitor : OsmiumParserBaseVisitor<object>
{
    public SymbolTable SymbolTable { get; private set; }

    public override object VisitFile([NotNull] OsmiumParser.FileContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        SymbolTable = new SymbolTable();

        if (context.program_block() is OsmiumParser.Program_blockContext program_block)
        {
            VisitProgram_block(program_block);
        }

        return 0;
    }

    public override object VisitProgram_block([NotNull] OsmiumParser.Program_blockContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        if (context.statement() is OsmiumParser.StatementContext[] statement_context)
        {
            foreach (var statement in statement_context)
            {
                VisitStatement(statement);
            }
        }

        if (context.expression() is OsmiumParser.ExpressionContext[] expression_contexts)
        {
            foreach (var expression in expression_contexts)
            {
                VisitExpression(expression);
            }
        }

        return null;
    }

    public override object VisitExpression([NotNull] OsmiumParser.ExpressionContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        // literal
        if (context.literal() is OsmiumParser.LiteralContext literal_context)
        {
            return VisitLiteral(literal_context);
        }

        // identifier
        if (context.identifier() is OsmiumParser.IdentifierContext identifier_context)
        {
            var identifier = identifier_context.GetText();
            if (SymbolTable.TryGetValue(identifier, out var value))
                return value;

            return null;
        }

        // invocation
        if (context.invocation() is OsmiumParser.InvocationContext invocation_context)
        {
            return VisitInvocation(invocation_context);
        }

        return null;
    }

    public override object VisitInvocation([NotNull] OsmiumParser.InvocationContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        var identifier = context.identifier().GetText();

        IList<object> parameters = null;

        if (context.expression_list() is OsmiumParser.Expression_listContext expression_list_context)
        {
            parameters = (IList<object>)VisitExpression_list(expression_list_context);
        }

        if ($"{identifier}" == "print")
        {
            if (parameters is null)
                return null;

            if (parameters.Count != 1)
                Log.Info($"invalid param count for print");

            Log.Info($"\n{parameters[0]}");
            //Log.Info();
        }

        return null;
    }

    public override object VisitExpression_list([NotNull] OsmiumParser.Expression_listContext context)
    {
        IList<object> returned_objects = null;
        if (context.expression() is OsmiumParser.ExpressionContext[] expression_contexts)
        {
            returned_objects = new List<object>();
            foreach (var expression in expression_contexts)
            {
                returned_objects.Add(VisitExpression(expression));
            }
        }

        // may be null
        return returned_objects;
    }

    public override object VisitStatement([NotNull] OsmiumParser.StatementContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        if (context.declaration() is OsmiumParser.DeclarationContext declaration_context)
        {
            VisitDeclaration(declaration_context);
        }

        return null;
    }

    public override object VisitDeclaration([NotNull] OsmiumParser.DeclarationContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        if (context.assignment() is OsmiumParser.AssignmentContext assignment_context)
        {
            VisitAssignment(assignment_context);
        }

        return null;
    }

    public override object VisitAssignment([NotNull] OsmiumParser.AssignmentContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        var identifier = context.identifier().GetText();

        if (SymbolTable.HasSymbol(identifier))
        {
            throw new InvalidOperationException($"Cannot re-define immutable identifier: {identifier}");
        }

        var value = VisitExpression(context.expression());

        SymbolTable[identifier] = value;

        return null;
    }

    public override object VisitLiteral([NotNull] OsmiumParser.LiteralContext context)
    {
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]}");

        // int
        if (context.@int() is OsmiumParser.IntContext int_context)
        {
            return int.Parse($"{context.sign()?.GetText()}{VisitInt(int_context)}");
        }

        // float
        if (context.@float() is OsmiumParser.FloatContext float_context)
        {
            return VisitFloat(float_context);
        }

        // double
        if (context.@double() is OsmiumParser.DoubleContext double_context)
        {
            return VisitDouble(double_context);
        }

        // char
        if (context.@char() is OsmiumParser.CharContext char_context)
        {
            return VisitChar(char_context);
        }

        // string
        if (context.@string() is OsmiumParser.StringContext string_context)
        {
            return VisitString(string_context);
        }

        // boolean
        if (context.boolean() is OsmiumParser.BooleanContext boolean_context)
        {
            return VisitBoolean(boolean_context);
        }

        // range
        if (context.range() is OsmiumParser.RangeContext range_context)
        {
            return VisitRange(range_context);
        }

        // null
        if (context.@null() is OsmiumParser.NullContext)
            return null;

        return null;
    }

    public override object VisitInt([NotNull] OsmiumParser.IntContext context)
    {
        var evaluate = int.Parse(context.GetText());
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");

        return evaluate;
    }

    public override object VisitFloat([NotNull] OsmiumParser.FloatContext context)
    {
        var evaluate = float.Parse(context.GetText().Trim('f'), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");

        return evaluate;
    }

    public override object VisitDouble([NotNull] OsmiumParser.DoubleContext context)
    {
        var evaluate = double.Parse(context.GetText(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");

        return evaluate;
    }

    public override object VisitChar([NotNull] OsmiumParser.CharContext context)
    {
        var content = context.GetText().Trim('\'');
        var evaluate = char.Parse(content);

        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");
        return evaluate;
    }

    public override object VisitString([NotNull] OsmiumParser.StringContext context)
    {
        var content = context.GetText().Trim('\"');
        var evaluate = new string(content);

        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");
        return evaluate;
    }

    public override object VisitBoolean([NotNull] OsmiumParser.BooleanContext context)
    {
        var evaluate = bool.Parse(context.GetText());

        Log.Info($":: {OsmiumParser.ruleNames[context.RuleIndex]} -> {evaluate}");
        return evaluate;
    }

    public override object VisitRange([NotNull] OsmiumParser.RangeContext context)
    {
        //throw new NotImplementedException();
        return null;
    }
}
