using DialogML.DNodes;
using DialogML.RNodes;
using System;
using System.Collections.Generic;

namespace DialogML
{
    enum XNodeType : byte
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
        Log = 10,
        OnceOnly = 11
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
        Finished,
        Continue,
        Parent
    }

    public enum ScriptEngineStatus
    {
        NoScriptRunning,
        RunningScript,
        PrepScript
    }

    public class ScriptEngine
    {
        public ScriptEngineStatus Status;

        //public AdvanceType RV;
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

        public void PrepScriptRecursive(RNode node)
        {
            node.Prep();
            foreach(var c in node.Children)
            {
                PrepScriptRecursive(c);
            }
        }

        public void PrepScript(CompiledScript script)
        {
            PrepScriptRecursive(script.Root);
        }

        public AdvanceType StartScript(CompiledScript script)
        {
            // Prep Script
            Status = ScriptEngineStatus.PrepScript;
            PrepScript(script);

            Status = ScriptEngineStatus.RunningScript;
            var currentNode = script.Root.Children[0];

            m_ProgramStack.Push(currentNode);
            m_IndexStack.Push(1);

            var RV = currentNode.Execute(m_ScriptApi);
            if(RV != AdvanceType.Yield &&
                RV != AdvanceType.Finished)
            {
                return Update(RV);
            }
            
            return RV;
         }

        internal void PushReturnParentNode()
        {
            throw new Exception("Don't call this method TODO Remove");
            m_ReturnStack.Push(m_ProgramStack.Count-1);
        }

        internal void PushReturnCurrentNode()
        {
            m_ReturnStack.Push(m_ProgramStack.Count);
        }
        
        public AdvanceType Update(AdvanceType advanceType = AdvanceType.Continue)
        {
            if (m_ProgramStack.Count == 0)
            {
                Status = ScriptEngineStatus.NoScriptRunning;
                return AdvanceType.Finished;
            }

            var currentNode = m_ProgramStack.Peek();
            var rv = advanceType;
           // AdvanceType rv = currentNode.Execute(m_ScriptApi);

            while(rv != AdvanceType.Yield &&
                  rv != AdvanceType.Finished)
            {
                // TODO I think I have done this main walk wrong.
                
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

                if (rv == AdvanceType.Parent)
                {
                    if(m_ProgramStack.Count > 0)
                    {
                        currentNode = m_ProgramStack.Pop();
                        m_IndexStack.Pop();
                    }
                    else
                    {
                        currentNode = null;
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
