grammar LabCalculator;

compileUnit : expression EOF ;

expression
    : LPAREN expression RPAREN                         # ParenthesesExpr
    | INC LPAREN expression RPAREN                     # IncExpr
    | DEC LPAREN expression RPAREN                     # DecExpr
    | op=(ADD|SUB) expression                          # UnaryExpr
    | left=expression op=EXP right=expression          # ExponentExpr
    | left=expression op=(MUL|DIV) right=expression    # MultiplicativeExpr
    | left=expression op=(ADD|SUB) right=expression    # AdditiveExpr
    | IDENTIFIER                                       # CellExpr
    | NUMBER                                           # NumberExpr
    ;

INC : 'inc' | 'INC' ;
DEC : 'dec' | 'DEC' ;
LPAREN : '(' ;
RPAREN : ')' ;
ADD    : '+' ;
SUB    : '-' ;
MUL    : '*' ;
DIV    : '/' ;
EXP    : '^' ;

IDENTIFIER : [A-Za-z]+ [0-9]+ ;
NUMBER     : [0-9]+ ('.' [0-9]+)? ;
WS         : [ \t\r\n]+ -> skip ;