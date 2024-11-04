lexer grammar OsmiumLexer;

// trimming
WHITESPACE				: [ \r\n\t]+ -> skip ;

//
// comments
//

COMMENT					: '//' ~[\r\n]* -> skip ;
MULTILINE_COMMENT		: '/*' .*? ('*/' | EOF) -> skip ;

//
// delimiters
//

LEFT_BRACKET			: '(' ;
RIGHT_BRACKET			: ')' ;

LEFT_SQUARE_BRACKET		: '[' ;
RIGHT_SQUARE_BRACKET	: ']' ;

LEFT_CURLY_BRACKET		: '{' ;
RIGHT_CURLY_BRACKET		: '}' ;

POINT			: '.' ;
COMMA			: ',' ;
APOSTROPHE		: '\'' ;
QUOTE			: '"' ;

SEMICOLON		: ';' ;
COLON			: ':' ;

//
// keywords
//

IN				: 'in' ;
IF				: 'if' ;
ELSE			: 'else' ;
FOR				: 'for' ;
DO				: 'do' ;
WHILE			: 'while' ;
ENUM			: 'enum' ;
LAMBDA			: 'lambda' ;
FUNCTION		: 'function' ;
RETURN			: 'return' ;
END				: 'end' ;
CONTINUE		: 'continue' ;
BREAK			: 'break' ;

NULL			: 'null' ;

//
// operators
//

OP_LOGICAL_NOT				: '!' ; // factorial? "!x, !5" or maybe inverse? "x!, 5!"
OP_LOGICAL_AND				: '&&' ;
OP_LOGICAL_OR				: '||' ;

OP_ADDITION					: '+' ;
OP_SUBTRACTION				: '-' ;
OP_MULTIPLY					: '*' ;
OP_DIVISION					: '/' ;
OP_MODULUS					: '%' ;

OP_BITWISE_AND				: '&' ;
OP_BITWISE_OR				: '|' ;
OP_EXCLUSIVE_OR				: '^' ;
OP_ONES_COMPLEMENT			: '~' ;
OP_LEFT_SHIFT				: '<<' ;
OP_RIGHT_SHIFT				: '>>' ;

OP_EQUALITY					: '==' ;
OP_INEQUALITY				: '!=' ;
OP_LESS_THAN				: '<' ;
OP_GREATER_THAN				: '>' ;
OP_LESS_THAN_OR_EQUALS		: '<=' ;
OP_GREATER_THAN_OR_EQUALS	: '>=' ;

OP_INCREMENT				: '++' ;
OP_DECREMENT				: '--' ;

OP_TRUE						: 'true' ;
OP_FALSE					: 'false' ;

OP_INDEX					: '[]' ;

OP_ASSIGN					: '=' ;

//
// values & variables
//

// numerics
fragment DIGIT			: '0'..'9' ;
//fragment HEX_PREFIX		: '0x' ;
//fragment HEX_INT		: HEX_PREFIX [0-9a-fA-F]+ ;

// int
INT
	: DIGIT+ 
	;

// float
//fragment FLOAT_EXPONENTIAL 
//	: (PURE_INT | PURE_FLOAT | DOUBLE ) 'e' (OP_ADD | OP_SUB)? (PURE_INT | PURE_FLOAT | DOUBLE) 
//	;

FLOAT 
	: DIGIT* POINT DIGIT+ 'f'?
	| DIGIT+ (POINT DIGIT+)? 'f'
	;

fragment LETTER : ('a' .. 'z') | ('A' .. 'Z') ;

// string
STRING : QUOTE ('""'|~'"')* QUOTE ;

// range
RANGE 
	: DIGIT+ POINT POINT DIGIT+		// 0..1
	| POINT POINT DIGIT+			// ..1
	| DIGIT+ POINT POINT			// 0..
	;

// identifier
VARIABLE : VALID_VAR_START VALID_VAR_CHAR* ;			// x, _y, a3
fragment VALID_VAR_START : LETTER | '_' ;				// var names start with a letter or _
fragment VALID_VAR_CHAR : VALID_VAR_START | DIGIT ;		// var name can contain numbers