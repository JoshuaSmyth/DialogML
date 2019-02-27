using DialogML.DNodes;
using DialogML.RNodes;
using System;
using System.Collections.Generic;

namespace DialogML
{
    enum XNodeType : ushort
    {
        Unknown = 0,
        Script = 1,
        Page = 2,
        Option = 3,
        OptionExit = 4,
        Say = 5,
        Select = 6,
        If = 7,
        CaseTrue = 8,
        CaseFalse = 9,
        Log = 10
    }

    enum VariableType
    {
        LocalVar = 0,
        GlobalVar = 1,
        LocalFlag = 2,
        GlobalFlag = 3,
    }

    public enum AdvanceType
    {
        Unknown,
        Yield,
        Next,
        FirstChild,
        SecondChild,
        ChildN,             // Look at register
        PopStack,           // i.e Goto Parent
        PushGoto,            // Look at jump register
        Finished,
        Continue,
        Repeat
    }

    public enum ScriptEngineStatus
    {
        NoScriptRunning,
        RunningScript
    }

    public class ScriptEngine
    {
        public ScriptEngineStatus Status;

        public Guid JumpRegister;
        public Int32 ChildNRegister;

        public StringTable m_StringTable = new StringTable();

        public Stack<RNode> m_ProgramStack = new Stack<RNode>();    // This is how we navigate the tree
        public Stack<int> m_IndexStack = new Stack<int>();          // Keep a record of what child index we are at.
        public Stack<int> m_ReturnStack = new Stack<int>();         // index into the program stack to change the default behaviour
                                                                    // of returning to the parent when reaching the end of the children.

        public ScriptApi m_ScriptApi;
        
        public ScriptEngine(StringTable stringTable)
        {
            m_StringTable = stringTable;
            m_ScriptApi = new ScriptApi(this, m_StringTable);
        }

        public AdvanceType StartScript(CompiledScript script)
        {
            Status = ScriptEngineStatus.RunningScript;
            var currentNode = script.Root.Children[0];

            m_ProgramStack.Push(currentNode);
            m_IndexStack.Push(1);

            var rv = currentNode.Execute(m_ScriptApi);
            if(rv != AdvanceType.Yield &&
                rv != AdvanceType.Finished)
            {
                return Update();
            }
            return rv;
         }

        internal void PushReturnCurrentNode()
        {
            m_ReturnStack.Push(m_ProgramStack.Count);
        }
        
        public AdvanceType Update()
        {
            if (m_ProgramStack.Count == 0)
            {
                Status = ScriptEngineStatus.NoScriptRunning;
                return AdvanceType.Finished;
            }

            var currentNode = m_ProgramStack.Peek();
            AdvanceType rv = currentNode.Execute(m_ScriptApi);

            while(rv != AdvanceType.Yield &&
                  rv != AdvanceType.Finished)
            {

                if (rv == AdvanceType.ChildN)
                {
                    int pushValue = 0;
                    currentNode = currentNode.Children[ChildNRegister];
                    m_ProgramStack.Push(currentNode);
                    m_IndexStack.Push(pushValue);
                }

                if(rv == AdvanceType.FirstChild)
                {
                    int pushValue = 0;
                    
                    currentNode = currentNode.Children[0];
                    m_ProgramStack.Push(currentNode);
                    m_IndexStack.Push(pushValue);
                }

                if(rv == AdvanceType.SecondChild)
                {
                    int pushValue = 0;

                    currentNode = currentNode.Children[1];
                    m_ProgramStack.Push(currentNode);
                    m_IndexStack.Push(pushValue);
                }

                if(rv == AdvanceType.Next)
                {
                    // Choose next sibling.
                    // If no siblings left, choose parents sibling.

                    currentNode = m_ProgramStack.Pop();
                    var index = m_IndexStack.Pop();
                    
                    if(m_ProgramStack.Count == 0)
                    {
                        rv = AdvanceType.Finished;
                        continue;
                    }

                    var parent = m_ProgramStack.Peek();
                   
                    index++;
                    if(index >= parent.Children.Count)
                    {
                        // Check the return stack if we need to enter a particular
                        // location.
                        if (m_ReturnStack.Count > 0)
                        {
                            var pindex = m_ReturnStack.Pop();
                            var indexValue = 0;
                            while(m_ProgramStack.Count > pindex)
                            {
                                m_ProgramStack.Pop();
                                indexValue = m_IndexStack.Pop();
                            }
                            currentNode = m_ProgramStack.Peek();
                            
                            rv = AdvanceType.Continue;
                            continue;
                        }


                        rv = AdvanceType.Next;
                        continue;
                    }
                    else
                    {
                        currentNode = parent.Children[index];
                        m_ProgramStack.Push(currentNode);
                        m_IndexStack.Push(index);
                    }
                }

                if(currentNode == null)
                {
                    rv = AdvanceType.Finished;
                }
                else
                {
                    rv = currentNode.Execute(m_ScriptApi);
                }
            }

            if(rv == AdvanceType.Finished)
            {
                Status = ScriptEngineStatus.NoScriptRunning;
            }
            return rv;
        }
    }
}
