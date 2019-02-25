using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class SemanticAnalyser
    {
        private readonly HostCallTable m_HostCallTable;
        private readonly HostSymbolTable m_HostSymbolTable;

        public SemanticAnalyser(HostCallTable hostCallTable)
        {
            m_HostCallTable = hostCallTable;
            m_HostSymbolTable = new HostSymbolTable();
        }

        public HostSymbolTable HostSymbolTable
        {
            get { return m_HostSymbolTable; }
        }

        public IEnumerable<SemanticToken> ApplySemantics(IEnumerable<Token> tokenStream)
        {
            var tokens = tokenStream.ToList();
            PreprocessSemantics(ref tokens);

            
            var rv = new List<SemanticToken>(tokens.Count());
            foreach (var token in tokens)
            {
                if (token.OperationType == OperationType.Operator)
                {
                    var t = OperatorToOpcode(token);
                    rv.Add(t);
                }
                else
                {
                    switch (token.TokenType)
                    {
                        case TokenType.FunctionCall:
                            {
                                var t = new SemanticToken
                                {
                                    TokenType = token.TokenType,
                                    OperationType = OperationType.FunctionCall,
                                    Precedence = 9
                                };

                                var function = m_HostCallTable.GetFunctionByName(token.TokenValue);
                                t.Data = function.Id;

                                rv.Add(t);
                                break;
                            }
                        case TokenType.FunctionArgumentSeperator:
                            {
                                rv.Add(new SemanticToken { TokenType = token.TokenType, OperationType = OperationType.Operand });
                                break;
                            }
                        case TokenType.Symbol:
                            {
                                var symbol = HostSymbolTable.GetSymbolByName(token.TokenValue);
                                rv.Add(new SemanticToken { TokenType = token.TokenType, OperationType = OperationType.Operator, Data = symbol.SymbolId, Precedence = 9});
                                break;
                            }
                        default:
                            {
                                var t = new SemanticToken
                                {
                                    TokenType = token.TokenType,
                                    OperationType = OperationType.Operand
                                };

                                if (!t.IsBracket())
                                    t.Data = Double.Parse(token.TokenValue);


                                rv.Add(t);
                                break;
                            }
                    }
                }
            }
            return rv;
        }

        private static SemanticToken OperatorToOpcode(Token token)
        {
            var t = new SemanticToken() { TokenType = token.TokenType, OperationType = token.OperationType };

            switch (t.TokenType)
            {
                case TokenType.Add:
                    t.Precedence = 6;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.Subtract:
                    t.Precedence = 6;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.Multiply:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.Divide:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.PowerOf:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case TokenType.UnaryMinus:
                    t.Precedence = 8;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case TokenType.Negation:
                    t.Precedence = 8;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case TokenType.Modulo:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.GreaterThan:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.GreaterThanOrEqualTo:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.LessThan:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.LessThanOrEqualTo:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.Equal:
                    t.Precedence = 4;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.NotEqual:
                    t.Precedence = 4;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.LogicalAnd:
                    t.Precedence = 3;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case TokenType.LogicalOr:
                    t.Precedence = 2;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;

                default:
                    throw new ExpressionParserException(String.Format("Unknown operator{0}", t));
                    break;
            }

            return t;
        }


        public void PreprocessSemantics(ref List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == TokenType.Subtract)
                {
                    if (tokens.Count - 1 < i + 1)
                        throw new Exception("Unexpected end of token stream");

                    if (tokens[i + 1].TokenType == TokenType.OpenBracket)
                        tokens[i].TokenType = TokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].OperationType == OperationType.Operator)
                        tokens[i].TokenType = TokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].TokenType == TokenType.OpenBracket)
                        tokens[i].TokenType = TokenType.UnaryMinus;

                    if (i == 0)
                        tokens[i].TokenType = TokenType.UnaryMinus;
                }
            }
        }
    }
}
