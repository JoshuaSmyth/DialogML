using System;
using System.Collections.Generic;
using System.IO;
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

            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("&&")), SemanticTokenType.LogicalAnd));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("||")), SemanticTokenType.LogicalOr));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape(">=")), SemanticTokenType.GreaterThanOrEqualTo));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("<=")), SemanticTokenType.LessThanOrEqualTo));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("==")), SemanticTokenType.Equal));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("!=")), SemanticTokenType.NotEqual));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape(">")), SemanticTokenType.GreaterThan));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("<")), SemanticTokenType.LessThan));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("^")), SemanticTokenType.PowerOf));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("/")), SemanticTokenType.Divide));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("%")), SemanticTokenType.Modulo));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("*")), SemanticTokenType.Multiply));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("+")), SemanticTokenType.Add));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("-")), SemanticTokenType.Subtract));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("!")), SemanticTokenType.Negation));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape("(")), SemanticTokenType.OpenBracket, OperationType.Operand));
            Tokenizer.AddToken(new InputToken(new Regex(Regex.Escape(")")), SemanticTokenType.CloseBracket, OperationType.Operand));
        }

        public void RegisterSymbol(String symbolName)
        {
            Tokenizer.RegisterSymbol(symbolName);
            SemanticAnalyser.HostSymbolTable.RegisterSymbol(symbolName, 0);
        }

        public Tokenizer Tokenizer
        {
            get { return m_Tokenizer; }
        }

        public SemanticAnalyser SemanticAnalyser
        {
            get { return m_SemanticAnalyser; }
        }


        public byte[] ConvertToBytestream(List<SemanticToken> tokens)
        {
            using(var ms = new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    bw.Write((byte) SemanticTokenType.StartStream);
                    bw.Write((byte) 1);

                    foreach(var t in tokens)
                    {
                        if(t.TokenType == SemanticTokenType.Symbol)
                        {
                            bw.Write((byte)t.TokenType);
                            bw.Write((Int32)t.Data);
                        }
                        else
                        {
                            if(t.IsNumber())
                            {
                                if(t.Data <= 255)
                                {
                                    var floor = Math.Floor(t.Data);
                                    if(Math.Abs(t.Data - floor) <= double.Epsilon)
                                    {
                                        // byte
                                        bw.Write((byte)SemanticTokenType.DecimalLiteral8);
                                        bw.Write((byte)floor);
                                    }
                                    else
                                    {
                                        // 32 but
                                        bw.Write((byte)SemanticTokenType.DecimalLiteral32);
                                        bw.Write(t.Data);
                                    }
                                }
                                else
                                {
                                    bw.Write((byte)SemanticTokenType.DecimalLiteral32);
                                    bw.Write(t.Data);
                                }
                            }
                            else
                            {
                                if(t.IsOperator())
                                {
                                    bw.Write((byte)t.TokenType);
                                }
                                else
                                {

                                    if(t.IsFunction())
                                    {
                                        bw.Write((byte)t.TokenType);
                                        bw.Write((Int32)t.Data);
                                    }
                                    else
                                    {


                                        throw new NotImplementedException();
                                    }
                                }
                            }
                        }
                    }

                    return ms.ToArray();
                }
            }
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
                                if (o.TokenType == SemanticTokenType.OpenBracket)
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
                                if (opcode.TokenType == SemanticTokenType.OpenBracket)
                                    stack.Push(opcode);

                                if (opcode.TokenType == SemanticTokenType.CloseBracket)
                                {
                                    var token = stack.Pop();
                                    while (stack.Count > 0 && token.TokenType != SemanticTokenType.OpenBracket)
                                    {
                                        rv.Add(token);
                                        token = stack.Pop();
                                    }

                                    if (token.TokenType != SemanticTokenType.OpenBracket)
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
