parser grammar OsmiumParser;

options { tokenVocab = OsmiumLexer; }

terminator: SEMICOLON ;

int : INT ;
float : FLOAT ;
double : DOUBLE ;
char : CHAR ;
string : STRING ;
boolean : BOOLEAN ;
range : RANGE ;
null : NULL ;

file : 
	program_block EOF // regular content
	;

// expressions are separated by semicolon
program_block : 
	(statement | expression) |
	(control_flow | ((statement | expression) terminator))+ 
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
	identifier ASSIGNMENT assignment | // <-- do not touch
	;

jump_statement :
	break_statement |
	continue_statement |
	return_statement
	;

control_flow : 
	scope |
	if_statement |
	for_statement |
	do_while_statement |
	while_statement
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

do_while_statement : 
	DO WHILE LEFT_BRACKET expression RIGHT_BRACKET COLON program_block END
	;

while_statement :
	WHILE LEFT_BRACKET expression RIGHT_BRACKET COLON program_block END
	;

break_statement : BREAK ;

continue_statement : CONTINUE ;

op_index : identifier LEFT_SQUARE_BRACKET (identifier | literal) RIGHT_SQUARE_BRACKET ;

// evaluates to a value
expression :
	literal |
	identifier |
	invocation |
	function_lambda | // anonymous implicit expression
	function_expression | // anonymous function
	LEFT_BRACKET expression RIGHT_BRACKET |

	op_index |

	// arithmetic
	expression OP_POW expression |
	expression OP_MUL expression |
	expression OP_DIV expression |
	expression OP_MOD expression |
	expression OP_ADD expression |
	expression OP_SUB expression |

	// boolean
	NOT expression |
	expression AND expression |
	expression OR expression |
	expression NOT_EQUALS expression |
	expression EQUALS expression |
	expression GREATER expression |
	expression LESS expression |
	expression GREATER_EQUALS expression |
	expression LESS_EQUALS expression
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
	sign? VARIABLE
	;

sign : (OP_ADD | OP_SUB)+;

literal : 
	sign? (double | float | int | char | string | boolean | null | range)
	;