using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public enum OperationType
    {
        Operand = 0,
        Operator = 1,
        FunctionCall = 2
    }

    public enum TokenDiscardPolicy
    {
        Keep = 0,
        Discard = 1
    }

    public enum OperatorAssociativity
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum TokenType
    {
        None = 0,

        // Operators
        Add = 1,
        Subtract = 2,
        Multiply = 3,
        Divide = 4,
        Assignment = 5,
        Negation = 6,
        PowerOf = 7,
        UnaryMinus = 10,
        Modulo = 11,
        GreaterThan = 12,
        LessThan = 13,
        Equal = 14,
        NotEqual = 15,
        GreaterThanOrEqualTo = 16,
        LessThanOrEqualTo = 17,
        LogicalAnd = 18,
        LogicalOr = 19,

        // Placeholders
        OpenBracket = 128,
        CloseBracket = 129,
        Whitespace = 130,
        FunctionArgumentSeperator = 131,

        // Operands
       // IntegerLiteral = 160,
        DecimalLiteral = 161,
        FunctionCall = 162,
        Symbol = 163
    }
}
