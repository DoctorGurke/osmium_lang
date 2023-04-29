parser grammar ArithmeticParser;

options { tokenVocab = ArithmeticLexer; }

terminator: SEMICOLON ;

double : DOUBLE ;
float : FLOAT ;
int : INT ;
char : CHAR ;
string : STRING ;
boolean : BOOLEAN ;
range : RANGE ;
null : NULL ;

file : 
	program_block EOF	// regular content
	;

// expressions are separated by semicolon
program_block : 
	(statement | expression) |
	((statement | expression) terminator)+ 
	;

//
// instructions
//

// does something
statement : 
	control_flow |
	declaration | 
	jump_statement
	;

declaration :
	function_declaration |
	assignment
	;

assignment : 
	identifier ASSIGNMENT expression |
	identifier ASSIGNMENT assignment | // multi assignment
	;

jump_statement :
	break_statement |
	continue_statement |
	return_statement |
	;

control_flow : 
	scope |
	if_statement |
	for_statement |
	while_statement |
	;

scope : LEFT_CURLY_BRACKET program_block RIGHT_CURLY_BRACKET ;

condition : expression ;

if_statement : IF LEFT_BRACKET condition RIGHT_BRACKET COLON program_block (else_if_statement* else_statement)? END;

else_if_statement : ELSE IF LEFT_BRACKET condition RIGHT_BRACKET COLON program_block ;

else_statement : 
	ELSE COLON program_block
	;

local_identifier : identifier ;

for_statement : 
	FOR LEFT_BRACKET local_identifier IN (identifier | literal) RIGHT_BRACKET COLON program_block END |
	FOR LEFT_BRACKET range RIGHT_BRACKET COLON program_block END |
	FOR LEFT_BRACKET int RIGHT_BRACKET COLON program_block END
	;

while_statement :
	WHILE LEFT_BRACKET expression RIGHT_BRACKET COLON program_block END
	;

break_statement : BREAK ;

continue_statement : CONTINUE ;

expression_target : identifier | expression ;

// evaluates to a value
expression :
	literal |
	identifier |
	invocation |
	function_lambda | // anonymous implicit expression
	function_expression | // anonymous function
	LEFT_BRACKET expression_target RIGHT_BRACKET |
	expression OP_POW expression_target |
	expression OP_MUL expression_target |
	expression OP_DIV expression_target |
	expression OP_MOD expression_target |
	expression OP_ADD expression_target |
	expression OP_SUB expression_target |

	NOT expression_target |
	expression AND expression_target |
	expression OR expression_target |
	expression NOT_EQUALS expression_target |
	expression EQUALS expression_target |
	expression GREATER expression_target |
	expression LESS expression_target |
	expression GREATER_EQUALS expression_target |
	expression LESS_EQUALS expression_target
	;

//
// function
//

// anonymous function implicitly evaluated as an expression
function_lambda : LAMBDA function_params? COLON expression? END ;

// anonymous function
function_expression : FUNCTION function_params? COLON program_block END ;
function_declaration : FUNCTION function_name function_params? COLON program_block END ;

function_name : identifier ;
function_params : LEFT_BRACKET (local_identifier (COMMA local_identifier)*)? RIGHT_BRACKET ;
	
return_statement : RETURN expression? ;

invocation : identifier LEFT_BRACKET expression_list?  RIGHT_BRACKET ;

//
// literal values
//

identifier_list : 
	identifier (COMMA identifier)* 
	;

expression_list : 
	expression (COMMA expression)*
	;

identifier : 
	VARIABLE |
	signed_identifier
	;

signed_identifier : (OP_ADD | OP_SUB)+ identifier ;

signed_literal : (OP_ADD | OP_SUB)+ (double | float | int | char) ;

literal : 
	signed_literal |
	(double | float | int | char | string | boolean | null | range)
	;