using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using static Osmium.Interpreter.OsmiumParser;

namespace Osmium.Interpreter;

public class Visitor : OsmiumParserBaseVisitor<object>
{
    public SymbolTable SymbolTable { get; private set; }

    private static void PrintContext(ParserRuleContext context, object value = null)
    {
        var pad = new StringBuilder();

        for (int i = 0; i < context.Depth() - 1; i++)
        {
            pad.Append("  ");
        }

        Log.Info($":: {pad}[] {ruleNames[context.RuleIndex]} {(value != null ? $" -> {value}" : "")}");
    }

    public override object VisitFile([NotNull] FileContext context)
    {
        PrintContext(context);

        SymbolTable = new SymbolTable();

        if (context.program_block() is Program_blockContext program_block)
        {
            VisitProgram_block(program_block);
        }

        return 0;
    }

    public override object VisitProgram_block([NotNull] Program_blockContext context)
    {
        PrintContext(context);

        if (context.statement() is StatementContext[] statement_context)
        {
            foreach (var statement in statement_context)
            {
                VisitStatement(statement);
            }
        }

        if (context.expression() is ExpressionContext[] expression_contexts)
        {
            foreach (var expression in expression_contexts)
            {
                VisitExpression(expression);
            }
        }

        return null;
    }

    public override object VisitExpression([NotNull] ExpressionContext context)
    {
        PrintContext(context, context.GetText());

        // literal
        if (context.literal() is LiteralContext literal_context)
        {
            return VisitLiteral(literal_context);
        }

        // identifier
        if (context.identifier() is IdentifierContext identifier_context)
        {
            var identifier = (string)VisitIdentifier(identifier_context);
            if (SymbolTable.TryGetValue(identifier, out var value))
                return value;

            return null;
        }

        // invocation
        if (context.invocation() is InvocationContext invocation_context)
        {
            return VisitInvocation(invocation_context);
        }

        //op_index
        //function_lambda
        //function_expression

        // bracket expression
        if (context.LEFT_BRACKET() is not null && context.RIGHT_BRACKET() is not null)
        {
            if (context.expression() is not null)
            {
                var expression = context.expression()[0];
                return VisitExpression(expression);
            }
        }

        return null;
    }

    public override object VisitInvocation([NotNull] InvocationContext context)
    {
        PrintContext(context);

        var identifier = (string)VisitIdentifier(context.identifier());

        IList<object> parameters = null;

        if (context.expression_list() is Expression_listContext expression_list_context)
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

    public override object VisitExpression_list([NotNull] Expression_listContext context)
    {
        IList<object> returned_objects = null;
        if (context.expression() is ExpressionContext[] expression_contexts)
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

    public override object VisitStatement([NotNull] StatementContext context)
    {
        PrintContext(context);

        if (context.declaration() is DeclarationContext declaration_context)
        {
            VisitDeclaration(declaration_context);
        }

        return null;
    }

    public override object VisitDeclaration([NotNull] DeclarationContext context)
    {
        PrintContext(context);

        if (context.assignment() is AssignmentContext assignment_context)
        {
            VisitAssignment(assignment_context);
        }

        return null;
    }

    public override object VisitAssignment([NotNull] AssignmentContext context)
    {
        var identifier = (string)VisitIdentifier(context.identifier());

        PrintContext(context, identifier);

        if (SymbolTable.HasSymbol(identifier))
        {
            throw new InvalidOperationException($"Cannot re-define immutable identifier: {identifier}");
        }

        var value = VisitExpression(context.expression());

        SymbolTable[identifier] = value;

        return null;
    }

    public override object VisitLiteral([NotNull] LiteralContext context)
    {
        PrintContext(context);

        // int
        if (context.@int() is IntContext int_context)
        {
            return int.Parse($"{context.sign()?.GetText()}{VisitInt(int_context)}");
        }

        // float
        if (context.@float() is FloatContext float_context)
        {
            return VisitFloat(float_context);
        }

        // double
        if (context.@double() is DoubleContext double_context)
        {
            return VisitDouble(double_context);
        }

        // char
        if (context.@char() is CharContext char_context)
        {
            return VisitChar(char_context);
        }

        // string
        if (context.@string() is StringContext string_context)
        {
            return VisitString(string_context);
        }

        // boolean
        if (context.boolean() is BooleanContext boolean_context)
        {
            return VisitBoolean(boolean_context);
        }

        // range
        if (context.range() is RangeContext range_context)
        {
            return VisitRange(range_context);
        }

        // null
        if (context.@null() is not null)
            return null;

        return null;
    }

    public override object VisitInt([NotNull] IntContext context)
    {
        var evaluate = int.Parse(context.GetText());

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitFloat([NotNull] FloatContext context)
    {
        var evaluate = float.Parse(context.GetText().Trim('f'), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitDouble([NotNull] DoubleContext context)
    {
        var evaluate = double.Parse(context.GetText(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitChar([NotNull] CharContext context)
    {
        var content = context.GetText().Trim('\'');
        var evaluate = char.Parse(content);

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitString([NotNull] StringContext context)
    {
        var content = context.GetText().Trim('\"');
        var evaluate = new string(content);

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitBoolean([NotNull] BooleanContext context)
    {
        var evaluate = bool.Parse(context.GetText());

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitRange([NotNull] RangeContext context)
    {
        //throw new NotImplementedException();
        return null;
    }

    public override object VisitIdentifier([NotNull] IdentifierContext context)
    {
        return $"{context.GetText()}";
    }
}
