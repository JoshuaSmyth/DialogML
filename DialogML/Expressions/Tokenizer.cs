using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressionParser
{
    public class Tokenizer
    {
        readonly List<InputToken> m_Tokens = new List<InputToken>();
        
        public Tokenizer()
        {
            m_Tokens.Add(new InputToken(new Regex(@"\s+"), SemanticTokenType.Whitespace, OperationType.Operand, TokenDiscardPolicy.Discard));
            m_Tokens.Add(new InputToken(new Regex("[0-9]+([.,][0-9]+)?"), SemanticTokenType.DecimalLiteral32, OperationType.Operand));
            m_Tokens.Add(new InputToken(new Regex(","), SemanticTokenType.FunctionArgumentSeperator, OperationType.Operand));  
        }

        public void AddToken(InputToken token)
        {
            m_Tokens.Add(token); // Add tokens in order of precedence
        }

        public List<InputToken> Tokenize(String source)
        {
            var rv = new List<InputToken>();

            var currentIndex = 0;
            while (currentIndex < source.Length)
            {
                var foundMatch = false;
                foreach (var token in m_Tokens)
                {
                   var match = token.Regex.Match(source, currentIndex);
                   if (match.Success && (match.Index - currentIndex) == 0)
                   {
                       if (token.DiscardPolicy == TokenDiscardPolicy.Keep)
                            rv.Add(new InputToken(token.Regex, token.TokenType, token.OperationType, token.DiscardPolicy) { TokenValue = match.Value});
                       
                       currentIndex += match.Length;
                       foundMatch = true;
                       break;
                   }
                }

                if (foundMatch == false)
                    throw new ExpressionParserException("Unknown token");
            }

            return rv;
        }



        public void RegisterSymbol(string symbolName)
        {
            var rg = new Regex(symbolName);
            foreach (var token in m_Tokens)
            {
                if (token.TokenType == SemanticTokenType.Symbol)
                {
                    if (token.Regex.ToString() == rg.ToString())
                        return;
                }
            }

            m_Tokens.Add(new InputToken(rg, SemanticTokenType.Symbol, OperationType.Operand));
        }

        public void ClearAllSymbols()
        {
            for (int i = 0; i < m_Tokens.Count; i++)
            {
                if (m_Tokens[i].TokenType == SemanticTokenType.Symbol)
                {
                    m_Tokens.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
