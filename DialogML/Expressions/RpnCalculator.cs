using System;
using System.Collections.Generic;
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
            RegisterFunction("cos", null, new Func<double, double>(Math.Cos));
            RegisterFunction("sin", null, new Func<double, double>(Math.Sin));
            RegisterFunction("exp", null, new Func<double, double>(Math.Exp));
            RegisterFunction("max", null, new Func<double, double, double>(Math.Max));
        }


        public EvaluationContext Compile(String input)
        {
            var instructions = m_Compiler.ConvertToReversePolishNotation(input);
            var symbolTable = (HostSymbolTable) m_Compiler.SemanticAnalyser.HostSymbolTable.Clone();
            return new EvaluationContext(symbolTable) { Instructions = instructions };
        }

        public void RegisterFunction(string name, object owner, Delegate function)
        {
            m_CallTable.RegisterFunction(name, function);
            m_Compiler.Tokenizer.AddToken(new Token(new Regex(name), TokenType.FunctionCall, OperationType.FunctionCall));
        }

        public double Evaluate(EvaluationContext context)
        {
            var opcodes = context.Instructions;
            var stack = context.EvaluationStack;
            var symbolTable = context.SymbolTable;
         
            foreach (var opcode in opcodes)
            {
                if (opcode.IsValue())
                {
                    stack.Push(opcode.Data);
                }
                else
                {
                    // Evaluate opcode
                    switch (opcode.TokenType)
                    {
                        case TokenType.Add:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs + rhs);
                                break;
                            }
                        case TokenType.Subtract:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs - rhs);
                                break;
                            }
                        case TokenType.Multiply:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs * rhs);
                                break;
                            }
                        case TokenType.Divide:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs / rhs);
                                break;
                            }
                        case TokenType.PowerOf:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(Math.Pow(lhs, rhs));
                                break;
                            }
                        case TokenType.UnaryMinus:
                            {
                                var rhs = stack.Pop();
                                stack.Push(rhs * -1);
                                break;
                            }
                        case TokenType.Modulo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs % rhs);
                                break;
                            }
                        case TokenType.Negation:
                            {
                                var rhs = stack.Pop();
                                stack.Push((rhs == 0) ? 1 : 0);
                                break;
                            }
                        case TokenType.GreaterThan:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs > rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LessThan:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs < rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.GreaterThanOrEqualTo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs >= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LessThanOrEqualTo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs <= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.Equal:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs == rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.NotEqual:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs != rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LogicalAnd:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var d1 = (rhs == 0) ? false : true;
                                var d2 = (lhs == 0) ? false : true;
                                var d3 = d1 && d2;
                                var data = (d3 == true) ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LogicalOr:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var d1 = (rhs == 0) ? false : true;
                                var d2 = (lhs == 0) ? false : true;
                                var d3 = d1 || d2;
                                var data = (d3 == true) ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.FunctionCall:
                            {
                                // TODO Might need a string table
                                var functionId = (Int32)opcode.Data;
                                var function = m_CallTable.GetFunctionById(functionId);
                                var parameters = function.ParameterList;
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    parameters[i] = stack.Pop();
                                }
                                var data = function.Invoke();
                                stack.Push(data);
                                break;
                            }
                        case TokenType.Symbol:
                            {
                                var symbolId = (Int32)opcode.Data;
                                var symbol = symbolTable.GetSymbolById(symbolId);
                                stack.Push(symbol.SymbolValue);
                                break;
                            }
                        default:
                            throw new Exception(String.Format("Unknown operator{0}",opcode));
                            break;
                    }
                }
            }

            // If more than one value on the stack error with input
            var r = stack.Pop();
            return r;
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
