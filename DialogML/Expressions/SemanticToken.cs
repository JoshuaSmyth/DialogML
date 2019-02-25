using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class SemanticToken // Ecapsulates the idea of an operator and an operand
    {
        public TokenType TokenType;

        public OperationType OperationType;

        public OperatorAssociativity OperatorAssociativity;

        public Int32 Precedence; // Higher evaluates first

        public double Data;

        public Int32 NumOperands; // 0, 1, 2 or more (or number of function parameters)

        public bool IsOperator()
        {
            return OperationType == OperationType.Operator;
        }

        public bool IsValue()
        {
            return OperationType == OperationType.Operand;
        }

        public bool IsNumber()
        {
            return IsValue() && (TokenType == TokenType.DecimalLiteral);
        }

        public bool IsBracket()
        {
            return (TokenType == TokenType.OpenBracket || TokenType == TokenType.CloseBracket);
        }

        public bool IsFunction()
        {
            return TokenType == TokenType.FunctionCall;
        }

        public bool IsFunctionArgumentSeperator()
        {
            return TokenType == TokenType.FunctionArgumentSeperator;
        }
    }
}
