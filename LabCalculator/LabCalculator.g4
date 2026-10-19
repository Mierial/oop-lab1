grammar LabCalculator;

compileUnit : expr EOF ;

expr
    : left=expr EXPONENT right=expr                           # ExponentExpr
    | left=expr op=(MULTIPLY | DIVIDE | DIV | MOD) right=expr # MultiplicativeExpr
    | left=expr op=(ADD | SUBTRACT) right=expr                # AdditiveExpr
    | MMAX '(' expr (',' expr)* ')'                           # MmaxExpr
    | MMIN '(' expr (',' expr)* ')'                           # MminExpr
    | '(' expr ')'                                            # ParenthesizedExpr
    | NUMBER                                                  # NumberExpr
    | CELL_INDEX                                              # CellExpr
    ;


EXPONENT    : '^' ;
MULTIPLY    : '*' ;
DIVIDE      : '/' ;
DIV         : 'div' ;
MOD         : 'mod' ;
ADD         : '+' ;
SUBTRACT    : '-' ;
MMAX        : 'mmax' ;
MMIN        : 'mmin' ;

NUMBER      : [0-9]+ ('.' [0-9]+)? ;
CELL_INDEX  : [A-Za-z]+ [0-9]+ ;
WS          : [ \t\r\n]+ -> skip ;