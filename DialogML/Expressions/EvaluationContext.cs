using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class EvaluationContext
    {
        /// <summary>
        /// A sequence of instructions to execute in RPN form
        /// </summary>
        public List<SemanticToken> Instructions { get; set; }

        /// <summary>
        /// The symbols to be resolved at execution time
        /// </summary>
        public HostSymbolTable SymbolTable { get; set; }

        /// <summary>
        /// The stack used by the evaluation process
        /// </summary>
        public Stack<Double> EvaluationStack { get; set; }

        public EvaluationContext(HostSymbolTable symbolTable)
        {
            SymbolTable = symbolTable;
            EvaluationStack = new Stack<Double>();
            Instructions = new List<SemanticToken>();
        }

        public void SetSymbol(string s, int i)
        {
           SymbolTable.RegisterSymbol(s,i);
        }
    }
}
