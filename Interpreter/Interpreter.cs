using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Osmium.Interpreter.Operators;
using System.Collections.Generic;
using System.Text;
using static Osmium.Interpreter.OsmiumParser;

namespace Osmium.Interpreter;

public class Interpreter : OsmiumParserBaseVisitor<object>
{
    public class ReturnException : Exception
    {
        public object Value { get; private set; }

        public ReturnException(object value)
        {
            Value = value;
        }
    }

    public SymbolTable SymbolTable { get; private set; }

    public Interpreter()
    {
        SymbolTable = new SymbolTable();
    }

    public Interpreter(SymbolTable parentSymbolTable)
    {
        SymbolTable = new SymbolTable();
        SymbolTable.Parent = parentSymbolTable;
    }

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

        try
        {
            if (context.program_block() is Program_blockContext program_block)
            {
                return VisitProgram_block(program_block);
            }
        }
        catch (ReturnException programReturn)
        {
            return programReturn.Value;
        }

        return null;
    }

    public override object VisitProgram_block([NotNull] Program_blockContext context)
    {
        PrintContext(context);

        // TODO: currently interprets program_block components based on priority
        // instead of actual order in code....

        if (context.children is not null)
            foreach (var child in context.children)
            {
                if (child is StatementContext statement)
                {
                    VisitStatement(statement);
                }
                else if (child is Control_flowContext control_flow)
                {
                    VisitControl_flow(control_flow);
                }
                else if (child is ExpressionContext expression)
                {
                    VisitExpression(expression);
                }
            }

        //if (context.control_flow() is Control_flowContext[] control_flow_contexts)
        //{
        //    foreach (var control_flow in control_flow_contexts)
        //    {
        //        VisitControl_flow(control_flow);
        //    }
        //}

        //if (context.statement() is StatementContext[] statement_context)
        //{
        //    foreach (var statement in statement_context)
        //    {
        //        VisitStatement(statement);
        //    }
        //}

        //if (context.expression() is ExpressionContext[] expression_contexts)
        //{
        //    foreach (var expression in expression_contexts)
        //    {
        //        VisitExpression(expression);
        //    }
        //}

        return null;
    }

    public override object VisitExpression([NotNull] ExpressionContext context)
    {
        if (context is null)
            return null;

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
            {
                PrintContext(identifier_context, value);
                return value;
            }

            return null;
        }

        // invocation
        if (context.invocation() is InvocationContext invocation_context)
        {
            return VisitInvocation(invocation_context);
        }

        // indexof
        if (context.op_index() is Op_indexContext op_index_context)
        {
            return VisitOp_index(op_index_context);
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

        if (context.op is not null)
        {
            //Log.Info($"{context.GetText()} {context.op} - {context.operand1} - {context.operand2}");
            // unary operation
            if (context.operand1 is not null && context.operand2 is null)
            {
                var operand = VisitExpression(context.operand1);
                return Arithmetic.TryUnary(context.op.Type, operand);
            }

            // binary operation
            if (context.operand1 is null || context.operand2 is null)
                return null;

            // get object values of expressions (recursively)
            var operand1 = VisitExpression(context.operand1);
            var operand2 = VisitExpression(context.operand2);
            //Log.Info($"operand2: {context.operand2.GetText()} {operand2}");

            //Log.Info($"operation: {context.op.Type} [{context.GetText()}]");
            // evaluate binary expression
            return Arithmetic.TryBinary(context.op.Type, operand1, operand2);
        }

        //Log.Info($"invalid expr: {context} {context.GetText()}");
        return null;
    }

    public override object VisitInvocation([NotNull] InvocationContext context)
    {
        PrintContext(context);

        var identifier = (string)VisitIdentifier(context.identifier());

        List<object> parameters = null;

        if (context.expression_list() is Expression_listContext expression_list_context)
        {
            parameters = (List<object>)VisitExpression_list(expression_list_context);
        }

        if ($"{identifier}" == "print")
        {
            if (parameters is null)
                return null;

            if (parameters.Count != 1)
                Log.Info($"invalid param count for print");

            var parameter = parameters[0];
            string printString = parameter?.ToString();

            if (parameter is List<object> list)
            {
                var sb = new StringBuilder();

                foreach (var item in list)
                {
                    sb.Append($"{item}, ");
                }

                printString = sb.ToString().Trim().Trim(',');
            }

            Log.Info($"\nprint::[{printString}]");
            //Log.Info();
        }

        if (SymbolTable.TryGetValue(identifier, out var symbol))
        {
            if (symbol is not Function func)
                throw new InvalidOperationException($"cannot invoke symbol: {identifier} : {symbol}");

            // new local interpreter for local symboltable => pure funcs
            var functionVisitor = new Interpreter();

            try
            {
                func.Invoke(functionVisitor, parameters?.ToArray());
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }
        }

        return null;
    }

    public override object VisitOp_index([NotNull] Op_indexContext context)
    {
        // TODO: this should be an operator so I can override it

        if (context.identifier() is IdentifierContext ident)
        {
            var identifier = (string)VisitIdentifier(ident);
            if (SymbolTable.TryGetValue(identifier, out var value))
                if (value is List<object> list)
                {
                    // direct index
                    if (context.@int() is IntContext index)
                    {
                        var intIndex = (int)VisitInt(index);
                        return list[intIndex];
                    }

                    // sublist
                    if (context.range() is RangeContext range)
                    {
                        var indexRange = (Range)VisitRange(range);
                        Log.Info($"RANGE: {indexRange}");
                        return GetSublist(list, indexRange);
                    }
                }
        }
        return null;
    }

    public static List<object> GetSublist(List<object> list, Range range)
    {
        int startIndex = range.StartIndex ?? 0;
        int endIndex = range.EndIndex ?? list.Count - 1;
        int count = endIndex - startIndex + 1;

        if (count < 0)
        {
            count = 0;
        }

        return list.GetRange(startIndex, count);
    }

    public override object VisitExpression_list([NotNull] Expression_listContext context)
    {
        List<object> returned_objects = null;
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

    public override object VisitIdentifier_list([NotNull] Identifier_listContext context)
    {
        if (context is null)
            return null;

        List<string> returned_objects = null;
        if (context.identifier() is IdentifierContext[] identifier_contexts)
        {
            returned_objects = new List<string>();
            foreach (var identifier in identifier_contexts)
            {
                returned_objects.Add((string)VisitIdentifier(identifier));
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

        if (context.jump_statement() is Jump_statementContext jump_statement_context)
        {
            VisitJump_statement(jump_statement_context);
        }

        if (context.control_flow() is Control_flowContext control_flow_context)
        {
            VisitControl_flow(control_flow_context);
        }

        return null;
    }

    public override object VisitJump_statement([NotNull] Jump_statementContext context)
    {
        PrintContext(context);

        if (context.return_statement() is Return_statementContext return_context)
        {
            throw new ReturnException(VisitExpression(return_context.expression()));
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

        if (context.function_declaration() is Function_declarationContext function_declaration_context)
        {
            VisitFunction_declaration(function_declaration_context);
        }

        return null;
    }

    public override object VisitFunction_declaration([NotNull] Function_declarationContext context)
    {
        PrintContext(context);

        var identifier = (string)VisitIdentifier(context.identifier());

        if (SymbolTable.HasSymbol(identifier))
        {
            throw new InvalidOperationException($"Cannot re-define immutable identifier: {identifier}");
        }

        var program = context.program_block();

        // lol
        var param_list = ((List<string>)VisitIdentifier_list(context.@params()?.identifier_list()))?.ToArray();

        SymbolTable[identifier] = new Function(identifier, program, param_list);

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
        Log.Info($"assign expr: {value}");
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

        if (context.list() is ListContext list_context)
        {
            return VisitList(list_context);
        }

        // null
        if (context.@null() is not null)
            return null;

        return null;
    }

    public override object VisitControl_flow([NotNull] Control_flowContext context)
    {
        PrintContext(context, context.GetText());

        var control_flow_visitor = new Interpreter(SymbolTable);

        if (context.if_statement() is If_statementContext statement)
        {
            {
                var condition = (bool)VisitCondition(statement.condition());
                //Log.Info($"condition: {condition}");
                if (condition)
                {
                    control_flow_visitor.VisitProgram_block(statement.program_block());
                    return null;
                }
            }

            if (statement.else_if_statement() is Else_if_statementContext[] else_if_statements)
            {
                foreach (var else_if in else_if_statements)
                {
                    var condition = (bool)VisitCondition(else_if.condition());
                    if (condition)
                    {
                        control_flow_visitor.VisitProgram_block(else_if.program_block());
                        return null;
                    }
                }
            }

            if (statement.else_statement() is Else_statementContext else_statement)
            {
                control_flow_visitor.VisitProgram_block(else_statement.program_block());
                return null;
            }
        }

        if (context.scope() is ScopeContext scope)
        {
            if (scope.program_block() is not null)
                control_flow_visitor.VisitProgram_block(scope.program_block());
        }

        return null;
    }

    public override object VisitCondition([NotNull] ConditionContext context)
    {
        PrintContext(context, context.GetText());

        return VisitExpression(context.expression());
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
        var evaluate = Range.Parse(context.GetText());

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitList([NotNull] ListContext context)
    {
        var list = new List<object>();
        if (context.expression_list() is Expression_listContext content)
        {
            var expressions = (List<object>)VisitExpression_list(context.expression_list());
            foreach (var expression in expressions)
            {
                list.Add(expression);
            }
        }

        return list;
    }

    public override object VisitIdentifier([NotNull] IdentifierContext context)
    {
        return $"{context.GetText()}";
    }
}
