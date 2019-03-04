using DialogML;
using DialogML.Expressions;
using ExpressionParser;
using System;
using System.Collections.Generic;

public class OptionSelection
{
    public List<Option> Options;

    public bool RemoveOnSelect;

    public bool OneSelectionOnly;

    public OptionSelection(List<Option> options)
    {
        Options = options;
    }
}

public class Option
{
    public bool IsExit;

    public Guid Id; // Or text?

    public Int32 ChildIndex;

    //public bool Visited;    // TODO
}

public class ScriptApi
{
    StringTable m_StringTable;
    ScriptEngine m_ScriptEngine;
    public ScriptApi(ScriptEngine scriptEngine, StringTable table)
    {
        m_ScriptEngine = scriptEngine;
        m_StringTable = table;
    }

    internal bool HasOnlyIfBeenExecuted(Guid id)
    {
        // TOOD IMplement
        return false;
    }

    // PushReturnToCurrentNode

    // PushReturnToParentNode

    // TODO Pass callback or ref ReturnFlag?
    // Or write to a register. (Stackable?)

    public void OnSelectOption(List<Option> options, Action<Option> SelectedOption)
    {
        var index = -1;

        while(index < 0 || index >= options.Count)
        {
            foreach(var o in options)
            {
                this.Trace(" - " + this.ResolveString(o.Id));
            }

            var key = Console.ReadKey();
            index = key.KeyChar - 49;
            this.Trace("");
        }

        SelectedOption(options[index]);
    }

    public void Say(String author, Guid textId) 
    {
        var output = m_StringTable.GetString(textId);
        Console.WriteLine(author + ": " + output);
    }

    internal void Trace(String output)
    {
        Console.WriteLine("trace: " + output);
    }

    public double EvaluateExpression(CompiledExpression expression)
    {
        // TODO Get these from a pool
        var rpnCalculator = new RpnCalculator();
        var hostTable = new HostSymbolTable();
        var stack = new Stack<Double>();

        return rpnCalculator.Evaluate(expression.Bytes, hostTable, stack);
    }

    internal string ResolveString(Guid id)
    {
        return m_StringTable.GetString(id);
    }

    internal void PushReturnParentNode()
    {
        m_ScriptEngine.PushReturnParentNode();
    }

    internal void PushReturnCurrentNode()
    {
        m_ScriptEngine.PushReturnCurrentNode();
    }

    internal void SetChildNRegister(int selectedIndex)
    {
        m_ScriptEngine.ChildNRegister = (selectedIndex-1);
    }
    
}