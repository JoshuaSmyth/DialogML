using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressionParser
{
    public class RpnCompiler
    {
        private readonly HostCallTable m_FunctionTable;
    //    private readonly HostSymbolTable m_HostSymbolTable;
        private readonly Tokenizer m_Tokenizer = new Tokenizer();
        private readonly SemanticAnalyser m_SemanticAnalyser;

        public RpnCompiler(HostCallTable functionTable)
        {
            m_FunctionTable = functionTable;
           // m_HostSymbolTable = hostSymbolTable;
            m_SemanticAnalyser = new SemanticAnalyser(m_FunctionTable);

            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("&&")), TokenType.LogicalAnd));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("||")), TokenType.LogicalOr));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape(">=")), TokenType.GreaterThanOrEqualTo));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("<=")), TokenType.LessThanOrEqualTo));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("==")), TokenType.Equal));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("!=")), TokenType.NotEqual));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape(">")), TokenType.GreaterThan));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("<")), TokenType.LessThan));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("^")), TokenType.PowerOf));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("/")), TokenType.Divide));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("%")), TokenType.Modulo));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("*")), TokenType.Multiply));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("+")), TokenType.Add));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("-")), TokenType.Subtract));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("!")), TokenType.Negation));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape("(")), TokenType.OpenBracket, OperationType.Operand));
            Tokenizer.AddToken(new Token(new Regex(Regex.Escape(")")), TokenType.CloseBracket, OperationType.Operand));
        }

        public Tokenizer Tokenizer
        {
            get { return m_Tokenizer; }
        }

        public SemanticAnalyser SemanticAnalyser
        {
            get { return m_SemanticAnalyser; }
        }


        public List<SemanticToken> ConvertToReversePolishNotation(String source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ExpressionParserException("Input string cannot be empty");

            var rv = new List<SemanticToken>();
            var tokenstream = Tokenizer.Tokenize(source);
            var opcodes = SemanticAnalyser.ApplySemantics(tokenstream);

            var stack = new Stack<SemanticToken>(tokenstream.Count);

            // Shunting Yard Algorithm
            // http://en.wikipedia.org/wiki/Shunting-yard_algorithm
            foreach (var opcode in opcodes)
            {
                if (opcode.IsNumber())
                {
                    rv.Add(opcode);
                }
                else
                {
                    if (opcode.IsFunction())
                    {
                        stack.Push(opcode);
                    }
                    else
                    {
                        if (opcode.IsFunctionArgumentSeperator())
                        {
                            while (stack.Count > 0)
                            {
                                var o = stack.Peek();
                                if (o.TokenType == TokenType.OpenBracket)
                                    break;

                                rv.Add(o);
                            }
                        }
                        else
                        {
                            if (opcode.IsOperator())
                            {
                                while (stack.Count > 0)
                                {
                                    var peek = stack.Peek();
                                    if ((opcode.Precedence < peek.Precedence) ||
                                        opcode.OperatorAssociativity == OperatorAssociativity.Left && opcode.Precedence == peek.Precedence) 
                                    {
                                        rv.Add(stack.Pop());
                                    }
                                    else { break; }
                                }
                     
                                stack.Push(opcode);
                            }
                            else
                            {
                                // Left Bracket
                                if (opcode.TokenType == TokenType.OpenBracket)
                                    stack.Push(opcode);

                                if (opcode.TokenType == TokenType.CloseBracket)
                                {
                                    var token = stack.Pop();
                                    while (stack.Count > 0 && token.TokenType != TokenType.OpenBracket)
                                    {
                                        rv.Add(token);
                                        token = stack.Pop();
                                    }

                                    if (token.TokenType != TokenType.OpenBracket)
                                        throw new ExpressionParserException("Mismatched brackets");
                                }
                            }
                        }
                    }
                }
            }

            // When there are no more tokens to read
            while (stack.Count > 0)
            {
                var op = stack.Pop();
                if (op.IsBracket())
                    throw new ExpressionParserException("Mismatched brackets");

                rv.Add(op);
            }

            return rv;
        }
    }
}
