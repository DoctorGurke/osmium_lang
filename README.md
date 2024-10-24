# osmium_lang
Custom interpreted functional programming language made with antlr4 and csharp.

# Main Features
- Immutable variables
- First-class Functions
- Line comments and comment blocks
- Base int, float, string, boolean, list and enum types
- C-Style Arithmetic/Boolean unary and binary operations
- Namespaces and scopes
- Built-in functions `print`, `length`, `foreach`, `map`, `reduce`, `filter`, `assert`

Targets .NET 7.0

Build and run OsmiumRuntime to run osmium in a terminal.
Interpreter, Parser and Lexer are in OsmiumInterpreter.

# Syntax Examples

### Comments
```
// Single-Line Comments
/*
 Multi-Line Comments
*/
```

### Base Types
```
float1 = 0.0;
float2 = -0f;
float3 = 10.0f * float1;

int1 = 1;
int2 = int1 + 4;

string1 = "Hello World!";

bool1 = true;
bool2 = !bool1;
bool3 = int1 > float1;
```

### Control Flow
```
if(condition1):
 /* */
else if(condition2):
 /* */
else:
 /* */
end
```

### List & Range
```
list1 = [1,2,3,4,5];
list2 = [
 "a",
 "b",
 "c"
];

// indexing starts at 0
value1 = list1[0]; // 1

// range operator
value2 = list1[0..]; // 1,2,3,4,5
value3 = list1[..2]; // 1,2,3
value3 = list1[1..3]; // 2,3,4
```

### Enum
```
// enums use list-style initializer and keyword
enum myEnum = [state1, state2, state3];
state = myEnum.state1; // 0

// allows for explicit value, duplicate value is invalid
// defaults to 0, iterating per undefined member
enum myCoolEnum = [
 state1 = 1,
 state2, // 0
 state3 = 100,
];

// get name from enum via indexof with enum value
name = myEnum[state]; // "state1"
```
### Functions
```
function foo(x): print(x); end;
func1 = function(x): /* */ end;
func2 = function: /* */ end;

// first-class citizen
function bar(func, arg):
 func(arg);
end;

bar(foo, "foobar"); // prints foobar

// return keyword for function returns
function mul(x, y):
 return x*y;
end;

val = mul(10, 10);
```

### Namespaces & Scopes
```
tools {
 enum error_type [
  error_unknown,
  error_severe,
  error_caught
 ];

 function error(err): 
  print(error_type[err]); 
 end;
};

{
 error = tools.error_type.error_unknown;
 tools.error(error); // prints "error_unknown"
};

{
 // no redefinition error due to local scopes
 error = tools.error_type.error_caught;
 tools.error(error); // prints "error_caught"
};
```

## More examples can be found under project/scripts.
