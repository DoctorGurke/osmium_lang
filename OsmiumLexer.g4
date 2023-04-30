lexer grammar OsmiumLexer;

// trimming

//NEWLINE : '\r'? '\n' | '\r' ;
WHITESPACE : [ \r\n\t]+ -> skip ;

//
// comments
//

COMMENT: '//' ~[\r\n]* -> skip ;
MULTILINE_COMMENT: '/*' .*? '*/' -> skip ;

//
// delimiters
//

LEFT_BRACKET : '(' ;
RIGHT_BRACKET : ')' ;

LEFT_SQUARE_BRACKET : '[' ;
RIGHT_SQUARE_BRACKET : ']' ;

LEFT_CURLY_BRACKET: '{' ;
RIGHT_CURLY_BRACKET: '}' ;

POINT : '.' ;
COMMA : ',' ;
APOSTROPHE : '\'' ;
QUOTE : '"' ;

SEMICOLON : ';' ;
COLON : ':' ;

//
// keywords
//

IN				: 'in' ;
IF				: 'if' ;
ELSE			: 'else' ;
FOR				: 'for' ;
DO				: 'do' ;
WHILE			: 'while' ;
LAMBDA			: 'lambda' ;
FUNCTION		: 'function' ;
RETURN			: 'return' ;
END				: 'end' ;
CONTINUE		: 'continue' ;
BREAK			: 'break' ;

fragment TRUE	: 'true' ;
fragment FALSE	: 'false' ;
BOOLEAN			: TRUE | FALSE ;

NULL			: 'null' ;

NOT_EQUALS		: '!=' ;
EQUALS			: '==' ;
GREATER			: '>' ;
LESS			: '<' ;
GREATER_EQUALS	: '>=' ;
LESS_EQUALS		: '<=' ;

NOT				: 'not' | '!' ; // factorial? "!x, !5" or maybe inverse? "x!, 5!"
AND				: 'and' | '&&' ;
OR				: 'or' | '||' ;

//
// storage
//

fragment LETTER : ('a' .. 'z') | ('A' .. 'Z') ;

ASSIGNMENT : '=' ;

ADD_ASSIGNMENT : '+=' ;
SUB_ASSIGNMENT : '-=' ;
MUL_ASSIGNMENT : '*=' ;
DIV_ASSIGNMENT : '/=' ;

// numerics

fragment DIGIT : '0'..'9' ;

fragment HEX_PREFIX : '0x' ;
fragment HEX_INT : HEX_PREFIX [0-9a-fA-F]+ ;
fragment PURE_INT : DIGIT+ ;

RANGE : 
	PURE_INT POINT POINT PURE_INT	| // int to int
	POINT POINT PURE_INT			| // start to int
	PURE_INT POINT POINT			  // int to end
	;

INT : 
	HEX_INT |
	PURE_INT 
	;

fragment FLOAT_EXPONENTIAL : (PURE_INT | PURE_FLOAT | DOUBLE ) 'e' (OP_ADD | OP_SUB)? (PURE_INT | PURE_FLOAT | DOUBLE) ;
fragment PURE_FLOAT : 
	DIGIT* POINT DIGIT+ 'f' |
	DIGIT+ 'f' 
	;
FLOAT : 
	PURE_FLOAT |
	FLOAT_EXPONENTIAL
	;

DOUBLE : DIGIT* POINT DIGIT+ ;

// super basic string and char, only a-z chars and numbers
CHAR: APOSTROPHE (LETTER | DIGIT)* APOSTROPHE;
STRING : QUOTE (LETTER | DIGIT)* QUOTE ;

VARIABLE : VALID_VAR_START VALID_VAR_CHAR* ;			// x, _y, a3
fragment VALID_VAR_START : LETTER | '_' ;				// var names start with a letter or _
fragment VALID_VAR_CHAR : VALID_VAR_START | DIGIT ;		// var name can contain numbers

// bitwise

// XOR			: '^' ;
// BIT_AND		: '&' ;
// BIT_OR		: '|' ;


fragment OPERATOR : (OP_POW | OP_MUL | OP_DIV | OP_MOD | OP_ADD | OP_SUB) ;

OP_POW : '**' ;
OP_MUL : '*' ;
OP_DIV : '/' ;
OP_MOD : '%' ;
OP_ADD : '+' ;
OP_SUB : '-' ;