﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ExpressionParser
{
    public class RpnCalculator
    {
    
        readonly RpnCompiler m_Compiler;

        readonly HostCallTable m_CallTable = new HostCallTable();

        public RpnCalculator()
        {
            m_Compiler = new RpnCompiler(m_CallTable);

            // Add some built in functions
            RegisterFunction("cos", new Func<double, double>(Math.Cos));
            RegisterFunction("sin", new Func<double, double>(Math.Sin));
            RegisterFunction("exp", new Func<double, double>(Math.Exp));
            RegisterFunction("max", new Func<double, double, double>(Math.Max));
        }


        public EvaluationContext Compile(String input)
        {
            var instructions = m_Compiler.ConvertToReversePolishNotation(input);
            var symbolTable = (HostSymbolTable) m_Compiler.SemanticAnalyser.HostSymbolTable.Clone();
            return new EvaluationContext(symbolTable) { Instructions = instructions };
        }

        public void RegisterFunction(string name, Delegate function)
        {
            var loweredName = name.ToLower();
            m_CallTable.RegisterFunction(loweredName, function);
            m_Compiler.Tokenizer.AddToken(new InputToken(new Regex(loweredName), SemanticTokenType.FunctionCall, OperationType.FunctionCall));
        }

        
        // TODO Add Evaluate RuntimeTokens
        public double Evaluate(byte[] tokenstream, HostSymbolTable symbolTable, Stack<Double> stack)
        {
            using(var ms = new MemoryStream(tokenstream))
            {
                using(var br = new BinaryReader(ms))
                {
                    var b1 = (SemanticTokenType) br.ReadByte();
                    if (b1 != SemanticTokenType.StartStream)
                    {
                        throw new Exception("Invalid first token. Expected StartStream: 255 was:" + b1);
                    }

                    var v1 = br.ReadByte();
                    if (v1 != 1)
                    {
                        throw new Exception("Error Invalid Version. Expected 1: Was:" + v1);
                    }

                    
                    while(ms.Position < ms.Length)
                    {
                        var token = (SemanticTokenType) br.ReadByte();
                        
                        {
                            switch(token)
                            {
                                // Numbers
                                case SemanticTokenType.DecimalLiteral32:
                                    stack.Push((double)br.ReadSingle());
                                    break;
                                case SemanticTokenType.DecimalLiteral16:
                                    stack.Push(br.ReadInt16());
                                    break;
                                case SemanticTokenType.DecimalLiteral8:
                                    stack.Push(br.ReadByte());
                                    break;

                                // Operators
                                case SemanticTokenType.Add:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(lhs + rhs);
                                        break;
                                    }
                                case SemanticTokenType.Subtract:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(lhs - rhs);
                                        break;
                                    }
                                case SemanticTokenType.Multiply:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(lhs * rhs);
                                        break;
                                    }
                                case SemanticTokenType.Divide:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(lhs / rhs);
                                        break;
                                    }
                                case SemanticTokenType.PowerOf:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(Math.Pow(lhs, rhs));
                                        break;
                                    }
                                case SemanticTokenType.UnaryMinus:
                                    {
                                        var rhs = stack.Pop();
                                        stack.Push(rhs * -1);
                                        break;
                                    }
                                case SemanticTokenType.Modulo:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        stack.Push(lhs % rhs);
                                        break;
                                    }
                                case SemanticTokenType.Negation:
                                    {
                                        var rhs = stack.Pop();
                                        stack.Push((rhs == 0) ? 1 : 0);
                                        break;
                                    }
                                case SemanticTokenType.GreaterThan:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs > rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.LessThan:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs < rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.GreaterThanOrEqualTo:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs >= rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.LessThanOrEqualTo:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs <= rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.Equal:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs == rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.NotEqual:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var data = (lhs != rhs) ? 1 : 0;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.LogicalAnd:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var d1 = (rhs == 0) ? false : true;
                                        var d2 = (lhs == 0) ? false : true;
                                        var d3 = d1 && d2;
                                        var data = (d3 == true) ? 1.0d : 0.0d;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.LogicalOr:
                                    {
                                        var rhs = stack.Pop(); var lhs = stack.Pop();
                                        var d1 = (rhs == 0) ? false : true;
                                        var d2 = (lhs == 0) ? false : true;
                                        var d3 = d1 || d2;
                                        var data = (d3 == true) ? 1.0d : 0.0d;
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.FunctionCall:
                                    {
                                        // TODO Might need a string table
                                        var functionId = (Int32)br.ReadInt32();
                                        var function = m_CallTable.GetFunctionById(functionId);
                                        var parameters = function.ParameterList;
                                        for(int i = 0; i < parameters.Length; i++)
                                        {
                                            parameters[i] = stack.Pop();
                                        }
                                        var data = function.Invoke();
                                        stack.Push(data);
                                        break;
                                    }
                                case SemanticTokenType.Symbol:
                                    {
                                        var symbolId = (Int32)br.ReadInt32(); // TODO Change to guid?
                                        var symbol = symbolTable.GetSymbolById(symbolId);
                                        stack.Push(symbol.SymbolValue);
                                        break;
                                    }
                                default:
                                    throw new Exception(String.Format("Unknown operator{0}", token));
                            }
                        }
                    }

                    if (stack.Count == 0)
                    {
                        return 0;
                    }

                    var r = stack.Pop();
                    return r;
                }
            }
        }


        public double Evaluate(EvaluationContext context)
        {
            var bytes = m_Compiler.ConvertToBytestream(context.Instructions);
            return Evaluate(bytes, context.SymbolTable, context.EvaluationStack);
        }

        public void RegisterSymbol(String symbolName)
        {
            m_Compiler.Tokenizer.RegisterSymbol(symbolName);
            m_Compiler.SemanticAnalyser.HostSymbolTable.RegisterSymbol(symbolName, 0);
        }
        
        public void ClearAllSymbols()
        {
            m_Compiler.Tokenizer.ClearAllSymbols();
        }
    }
}
