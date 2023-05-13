parser grammar OsmiumParser;

options { tokenVocab = OsmiumLexer; }

terminator: SEMICOLON ;

int : INT ;
float : FLOAT ;
string : STRING ;
boolean : OP_TRUE | OP_FALSE ;
range : RANGE ;
null : NULL ;
list : OP_INDEX | LEFT_SQUARE_BRACKET expression_list RIGHT_SQUARE_BRACKET ;

file 
	: program_block EOF // regular content
	;

// expressions are separated by semicolon
program_block 
	: (statement | expression) 
	| (control_flow | ((statement | expression) terminator))+
	|
	;

//
// instructions
//

// does something
statement 
	: control_flow 
	| declaration 
	| jump_statement
	;

declaration 
	: function_declaration
	| assignment
	;

assignment
	: identifier OP_ASSIGN expression
	| identifier OP_ASSIGN assignment
	;

jump_statement
	: break_statement
	| continue_statement
	| return_statement
	;

control_flow 
	: scope 
	| if_statement 
	| for_statement 
	| do_while_statement 
	| while_statement
	;

scope 
	: LEFT_CURLY_BRACKET program_block RIGHT_CURLY_BRACKET 
	;

condition 
	: expression 
	;

if_statement 
	: IF LEFT_BRACKET condition RIGHT_BRACKET COLON program_block (else_if_statement* else_statement)? END
	;

else_if_statement 
	: ELSE IF LEFT_BRACKET condition RIGHT_BRACKET COLON program_block 
	;

else_statement 
	: ELSE COLON program_block
	;

local_identifier 
	: identifier 
	;

for_statement 
	: FOR LEFT_BRACKET local_identifier IN (identifier | literal) RIGHT_BRACKET COLON program_block END 
	| FOR LEFT_BRACKET range RIGHT_BRACKET COLON program_block END 
	| FOR LEFT_BRACKET int RIGHT_BRACKET COLON program_block END
	;

do_while_statement 
	: DO WHILE LEFT_BRACKET expression RIGHT_BRACKET COLON program_block END
	;

while_statement 
	: WHILE LEFT_BRACKET expression RIGHT_BRACKET COLON program_block END
	;

break_statement 
	: BREAK 
	;

continue_statement 
	: CONTINUE 
	;

return_statement : RETURN expression? ;

op_index 
	: identifier LEFT_SQUARE_BRACKET (int | range) RIGHT_SQUARE_BRACKET
	;

// evaluates to a value
expression 
	: literal 
	| identifier 
	| invocation 
	| op_index // indexof
	| function_lambda // anonymous implicit expression
	| function_expression // anonymous function 

	| LEFT_BRACKET expression RIGHT_BRACKET 
	
	// arithmetic
	| operand1=expression op=OP_MULTIPLY operand2=expression 
	| operand1=expression op=OP_DIVISION operand2=expression 
	| operand1=expression op=OP_MODULUS operand2=expression 
	| operand1=expression op=OP_ADDITION operand2=expression 
	| operand1=expression op=OP_SUBTRACTION operand2=expression 
	
	// boolean
	| op=OP_LOGICAL_NOT operand1=expression 
	| operand1=expression op=OP_LOGICAL_AND operand2=expression 
	| operand1=expression op=OP_LOGICAL_OR operand2=expression 
	| operand1=expression op=OP_INEQUALITY operand2=expression 
	| operand1=expression op=OP_EQUALITY operand2=expression 
	| operand1=expression op=OP_GREATER_THAN operand2=expression 
	| operand1=expression op=OP_LESS_THAN operand2=expression 
	| operand1=expression op=OP_GREATER_THAN_OR_EQUALS operand2=expression 
	| operand1=expression op=OP_LESS_THAN_OR_EQUALS operand2=expression
	;

//
// function
//

// anonymous function implicitly evaluated as an expression
function_lambda 
	: LAMBDA params? COLON expression? END 
	;

// anonymous function
function_expression 
	: FUNCTION params? COLON program_block END 
	;
function_declaration 
	: FUNCTION identifier params? COLON program_block END 
	;

params 
	: LEFT_BRACKET (local_identifier (COMMA local_identifier)*)? RIGHT_BRACKET 
	;
	
invocation 
	: identifier LEFT_BRACKET expression_list?  RIGHT_BRACKET 
	;

//
// literal values
//

identifier_list 
	: identifier (COMMA identifier)* 
	;

expression_list 
	: expression (COMMA expression)*
	;

identifier 
	: sign? VARIABLE
	;

sign 
	: (OP_ADDITION | OP_SUBTRACTION)+
	;

literal 
	: sign? ( float | int | string | boolean | null | range | list)
	;