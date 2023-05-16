#ifndef NODE_H
#define NODE_H

enum class Type { Operand, Operation, Function, Bracket };

enum class Priority { None, AddSub, MulDivMod, Pow, Function };

enum class Operation { None, Add, Sub, Mul, Div, Mod, Pow };

enum class Function { None, Cos, Acos, Sin, Asin, Tan, Atan, Log, Log10, Sqrt };

enum class Bracket { None, OpenBracket, CloseBracket };

struct Node
{
    double operand;
    Operation operation;
    Function function;
    Bracket bracket;
};

#endif // !NODE_H
