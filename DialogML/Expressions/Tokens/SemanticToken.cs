using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class SemanticToken // Ecapsulates the idea of an operator and an operand
    {
        public SemanticTokenType TokenType;

        public OperationType OperationType;

        public OperatorAssociativity OperatorAssociativity;

        public Int32 Precedence; // Higher evaluates first

        public float Data; // THis could probably be a float

      //  public Int32 NumOperands; // 0, 1, 2 or more (or number of function parameters)

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
            return IsValue() && (TokenType == SemanticTokenType.DecimalLiteral32);
        }

        public bool IsBracket()
        {
            return (TokenType == SemanticTokenType.OpenBracket || TokenType == SemanticTokenType.CloseBracket);
        }

        public bool IsFunction()
        {
            return TokenType == SemanticTokenType.FunctionCall;
        }

        public bool IsFunctionArgumentSeperator()
        {
            return TokenType == SemanticTokenType.FunctionArgumentSeperator;
        }
    }
}
