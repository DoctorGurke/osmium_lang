using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Osmium.Interpreter.Operators;
using Osmium.Interpreter.Types;
using System.Text;
using static Osmium.Interpreter.OsmiumParser;
using static Osmium.Interpreter.Runtime;

namespace Osmium.Interpreter;

public class Interpreter : OsmiumParserBaseVisitor<object>, IMembers
{
    public class ReturnException : Exception
    {
        public object? Value { get; private set; }

        public ReturnException(object? value)
        {
            Value = value;
        }
    }

    private SymbolTable SymbolTable { get; set; }

    public Interpreter()
    {
        SymbolTable = new SymbolTable();
    }

    public static bool Debug { get; set; } = false;

    public SymbolTable Members => SymbolTable;

    public Interpreter(SymbolTable parentSymbolTable)
    {
        SymbolTable = new SymbolTable(parentSymbolTable);
    }

    /// <summary>
    /// Overrides the symbol table to output to an external reference.
    /// </summary>
    /// <param name="outputTable"></param>
    public void OverrideSymbolTable(SymbolTable outputTable)
    {
        SymbolTable = outputTable;
    }

    private static void PrintContext(ParserRuleContext context, object? value = null)
    {
        if (!Debug || context is null)
            return;

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
                VisitProgram_block(program_block);
            }
        }
        catch (ReturnException programReturn)
        {
            if (programReturn.Value is not null)
                return programReturn.Value;
        }

        return null;
    }

    public new object? VisitProgram_block([NotNull] Program_blockContext context)
    {
        PrintContext(context);

        return VisitChildren(context);
    }

    public override object VisitDirect_expression([NotNull] Direct_expressionContext context)
    {
        throw new ReturnException(VisitChildren(context));
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

            var value = SymbolTable.GetSymbolValue(identifier);

            PrintContext(identifier_context, value);
            return value;
        }

        // invocation
        if (context.invocation() is InvocationContext invocation_context)
        {
            return VisitInvocation(invocation_context);
        }

        if (context.op_member() is Op_memberContext op_member_context)
        {
            return VisitOp_member(op_member_context);
        }

        // indexof
        if (context.op_index() is Op_indexContext op_index_context)
        {
            return VisitOp_index(op_index_context);
        }

        if (context.function_lambda() is Function_lambdaContext lambda_context)
        {
            return VisitFunction_lambda(lambda_context);
        }

        if (context.function_expression() is Function_expressionContext function_expression_context)
        {
            return VisitFunction_expression(function_expression_context);
        }

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
                return Arithmetic.TryUnary(context.op.Type, operand ?? false);
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

        throw new NotImplementedException($"Invalid Expression!");
    }



    public new object? VisitInvocation([NotNull] InvocationContext context)
    {
        PrintContext(context);

        var identifier = (string)VisitIdentifier(context.identifier());

        object[] parameters = Array.Empty<object>();

        if (context.expression_list() is Expression_listContext expression_list_context)
        {
            parameters = (object[])VisitExpression_list(expression_list_context);
        }

        // TODO: find intrinsics with attribute?
        switch (identifier)
        {
            case "print":
                Intrinsics.Print(parameters);
                return null;
            case "length":
                return Intrinsics.Length(parameters);
            case "foreach":
                Intrinsics.ForEach(parameters);
                return null;
            case "map":
                return Intrinsics.Map(parameters);
            case "reduce":
                return Intrinsics.Reduce(parameters);
            case "filter":
                return Intrinsics.Filter(parameters);
            case "assert":
                Intrinsics.Assert(parameters);
                return null;
        }

        var value = SymbolTable.GetSymbolValue(identifier);

        if (value is not Function func)
            throw new InvalidOperationException($"Cannot invoke symbol: '{identifier}' : {value.GetType()}!");

        // new local interpreter for local symboltable => pure funcs
        var functionVisitor = new Interpreter();

        return func.Invoke(functionVisitor, parameters);
    }

    /// <summary>
    /// Evaluate member.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override object VisitOp_member([NotNull] Op_memberContext context)
    {
        var identifier = context.VARIABLE().GetText();

        var obj = SymbolTable.GetSymbolValue(identifier);

        PrintContext(context, $"{identifier} {obj}");

        if (obj is not IMembers source)
            throw new RuntimeException($"Trying to access member of invalid type {obj.GetType()} for {identifier}!");

        if (context.op_member() is Op_memberContext op_MemberContext)
        {
            var member_visitor = new Interpreter(source.Members);
            return member_visitor.VisitOp_member(op_MemberContext);
        }
        else if (context.member() is MemberContext member_context)
        {
            var member = (string)VisitMember(member_context);

            var value = source.Members.GetSymbolValue(member);

            return value;
        }
        else if (context.member_invocation() is Member_invocationContext member_InvocationContext)
        {
            return VisitMember_invocation(member_InvocationContext, source);
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Return string name of member
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override object VisitMember([NotNull] MemberContext context)
    {
        return context.VARIABLE().GetText();
    }

    /// <summary>
    /// Return evaluated member invocation.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public object VisitMember_invocation([NotNull] Member_invocationContext context, IMembers source)
    {
        var identifier = context.VARIABLE().GetText();

        var obj = source.Members.GetSymbolValue(identifier);

        if (obj is not IFunction func)
            throw new RuntimeException($"Trying to invoke invalid type {obj.GetType()} {identifier}!");

        object[] parameters = Array.Empty<object>();

        if (context.expression_list() is Expression_listContext expression_ListContext)
        {
            parameters = (object[])VisitExpression_list(expression_ListContext);
        }

        var function_visitor = new Interpreter();
        return func.Invoke(function_visitor, parameters);
    }

    public override object VisitOp_index([NotNull] Op_indexContext context)
    {
        // TODO: this should be an operator so I can override it

        if (context.identifier() is not IdentifierContext ident)
            return null;

        var identifier = (string)VisitIdentifier(ident);
        var value = SymbolTable.GetSymbolValue(identifier);

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
                var indexRange = (Types.Range)VisitRange(range);
                //Log.Info($"RANGE: {indexRange}");
                return GetSublist(list, indexRange);
            }
        }
        else if (value is Types.Enum members)
        {
            // direct index
            if (context.@int() is IntContext index)
            {
                var intIndex = (int)VisitInt(index);
                return members.GetIndexOf(intIndex);
            }
        }

        return null;
    }

    public static List<object> GetSublist(List<object> list, Types.Range range)
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

    public override object VisitFunction_lambda([NotNull] Function_lambdaContext context)
    {
        PrintContext(context);

        var expression = context.expression();

        string[] param_list = Array.Empty<string>();

        if (context.@params()?.identifier_list() is Identifier_listContext list)
        {
            param_list = (string[])VisitIdentifier_list(list);
        }

        return new Lambda(expression, param_list);
    }

    public override object VisitFunction_expression([NotNull] Function_expressionContext context)
    {
        PrintContext(context);

        var program = context.program_block();

        string[] param_list = Array.Empty<string>();

        if (context.@params()?.identifier_list() is Identifier_listContext list)
        {
            param_list = (string[])VisitIdentifier_list(list);
        }

        var func = new Function(null, program, param_list);

        return func;
    }

    /// <summary>
    /// Returns expression list as object array.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override object VisitExpression_list([NotNull] Expression_listContext context)
    {
        PrintContext(context);

        List<object> returned_objects = new();
        if (context.expression() is ExpressionContext[] expression_contexts)
        {
            returned_objects = new List<object>();
            foreach (var expression in expression_contexts)
            {
                returned_objects.Add(VisitExpression(expression));
            }
        }

        return returned_objects.ToArray();
    }

    /// <summary>
    /// Visit an identifier list.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Returns string[] of identifiers. Empty array if none are parsed.</returns>
    public override object VisitIdentifier_list([NotNull] Identifier_listContext context)
    {
        PrintContext(context);

        if (context is null)
            return Array.Empty<string>();

        List<string> returned_objects = new();
        if (context.identifier() is IdentifierContext[] identifier_contexts)
        {
            returned_objects = new List<string>();
            foreach (var identifier in identifier_contexts)
            {
                returned_objects.Add((string)VisitIdentifier(identifier));
            }
        }

        return returned_objects.ToArray();
    }

    public override object VisitStatement([NotNull] StatementContext context)
    {
        PrintContext(context);

        return VisitChildren(context);
    }

    public override object VisitJump_statement([NotNull] Jump_statementContext context)
    {
        PrintContext(context);

        if (context.return_statement() is Return_statementContext return_context)
        {
            if (return_context.expression() is null)
                throw new ReturnException(null);

            throw new ReturnException(VisitExpression(return_context.expression()));
        }

        return null;
    }

    public override object VisitDeclaration([NotNull] DeclarationContext context)
    {
        PrintContext(context);

        return VisitChildren(context);
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

        string[] param_list = Array.Empty<string>();

        if (context.@params()?.identifier_list() is Identifier_listContext list)
        {
            param_list = (string[])VisitIdentifier_list(list);
        }

        var func_declaration = new Function(identifier, program, param_list);

        SymbolTable.SetSymbol(identifier, func_declaration);

        return func_declaration;
    }

    private struct Member
    {
        public string Name;
        public object? Value;

        public Member(string name, object? value)
        {
            Name = name;
            Value = value;
        }
    }

    public override object VisitEnum_declaration([NotNull] Enum_declarationContext context)
    {
        var identifier = (string)VisitIdentifier(context.identifier());

        PrintContext(context, identifier);

        if (SymbolTable.HasSymbol(identifier))
        {
            throw new InvalidOperationException($"Cannot re-define immutable identifier: {identifier}");
        }

        Dictionary<string, int?> members = new();
        if (context.enum_member_list() is Enum_member_listContext enum_member_list_context)
        {
            var enum_members = (List<Member>)VisitEnum_member_list(enum_member_list_context);

            foreach (var member in enum_members)
            {
                members.Add(member.Name, (int?)member.Value);
            }
        }

        var enum_declaration = new Types.Enum(identifier, members);

        SymbolTable.SetSymbol(identifier, enum_declaration);

        return enum_declaration;
    }

    public override object VisitEnum_member_list([NotNull] Enum_member_listContext context)
    {
        PrintContext(context);

        List<Member> members = new();
        if (context.enum_member() is Enum_memberContext[] enum_members)
        {
            foreach (var enum_member in enum_members)
            {
                members.Add((Member)VisitEnum_member(enum_member));
            }
        }

        return members;
    }

    public override object VisitEnum_member([NotNull] Enum_memberContext context)
    {
        var identifier = (string)VisitIdentifier(context.identifier());

        int? value = null;

        if (context.@int() is IntContext int_context)
        {
            value = (int)VisitInt(int_context);
        }

        PrintContext(context, $"{identifier} {value}");

        return new Member(identifier, value);
    }

    public override object VisitAssignment([NotNull] AssignmentContext context)
    {
        PrintContext(context);
        var identifier = (string)VisitIdentifier(context.identifier());

        if (SymbolTable.HasSymbol(identifier))
        {
            throw new InvalidOperationException($"Cannot re-define immutable identifier: {identifier}");
        }

        var value = VisitExpression(context.expression());
        //Log.Info($"assign expr: {value}");
        SymbolTable.SetSymbol(identifier, value);

        return null;
    }

    public override object VisitNamespace_declaration([NotNull] Namespace_declarationContext context)
    {
        var identifier = (string)context.VARIABLE().GetText();

        if (SymbolTable.HasSymbol(identifier))
            throw new RuntimeException($"Invalid redefinition of identifier {identifier}!");

        PrintContext(context, identifier);

        var namespace_visitor = new Interpreter();
        namespace_visitor.VisitProgram_block(context.scope().program_block());

        SymbolTable.SetSymbol(identifier, namespace_visitor.Members);

        return namespace_visitor.Members;
    }

    public override object VisitLiteral([NotNull] LiteralContext context)
    {
        PrintContext(context);

        return VisitChildren(context);
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
            control_flow_visitor.VisitScope(scope);
        }

        return null;
    }

    public override object VisitScope([NotNull] ScopeContext context)
    {
        PrintContext(context);
        return VisitProgram_block(context.program_block());
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
        var evaluate = Types.Range.Parse(context.GetText());

        PrintContext(context, evaluate);
        return evaluate;
    }

    public override object VisitList([NotNull] ListContext context)
    {
        var list = new List<object>();
        if (context.expression_list() is Expression_listContext content)
        {
            var expressions = (object[])VisitExpression_list(content);
            list = expressions.ToList();
        }

        PrintContext(context, list);

        return list;
    }

    /// <summary>
    /// Visit identifier.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>String of identifier name.</returns>
    public override object VisitIdentifier([NotNull] IdentifierContext context)
    {
        var identifier = $"{context.GetText()}";

        PrintContext(context, identifier);
        return identifier;
    }
}
