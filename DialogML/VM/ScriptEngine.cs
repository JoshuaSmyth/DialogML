using DialogML.DNodes;
using DialogML.RNodes;
using DialogML.VM;
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
        OnceOnly = 11,
        Return = 12,
        Wait = 13,
        CallPage = 14,
        Exit = 15,
        Parallel = 16,
        ParallelUnit = 17,
        Sequential = 18,
        Custom = 255
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
        Parent,
        JumpToNode,
        Return
    }

    public enum ScriptEngineStatus
    {
        NoScriptRunning,
        RunningScript,
        PrepScript
    }

    public class OnlyIfTable
    {
        HashSet<Guid> visted = new HashSet<Guid>();

        public void MarkVisited(Guid id)
        {
            visted.Add(id);
        }

        public bool HasOnlyIfBeenExecuted(Guid id)
        {
            return visted.Contains(id);
        }
    }

    public class ExecutionUnit
    {
        public Guid JumpRegister;
        public Int32 ChildNRegister;

        public Stack<RNode> ProgramStack = new Stack<RNode>();    // This is how we navigate the tree
        public Stack<int> IndexStack = new Stack<int>();          // Keep a record of what child index we are at.
        public Stack<int> ReturnStack = new Stack<int>();         // index into the program stack to change the default behaviour
                                                                    // of returning to the parent when reaching the end of the children.

    }

    public class ScriptEngine
    {
        public ScriptEngineStatus Status;

        // TODO Make Private
        private ExecutionUnit ExecutionUnit = new ExecutionUnit();
        
        // TODO Extract into ProgramTable
        public OnlyIfTable m_OnlyIfTable = new OnlyIfTable();
        public StringTable m_StringTable = new StringTable();
        public RuntimeReferencesTable m_ReferencesTable = new RuntimeReferencesTable();

        // TODO Extract into execution units
        public ScriptApi m_ScriptApi;
        
        public ScriptEngine(RuntimeReferencesTable referencesTable, StringTable stringTable, OnlyIfTable onlyIfTable)
        {
            m_StringTable = stringTable;
            m_OnlyIfTable = onlyIfTable;
            m_ReferencesTable = referencesTable;
            m_ScriptApi = new ScriptApi(this, m_StringTable);
        }

        internal void SetChildNRegister(int n)
        {
            ExecutionUnit.ChildNRegister = n;
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
            // TODO Populate the references table
            // (Maybe this should be done at a loadscript stage)
            m_ReferencesTable.AddOrUpdateScript(script);

            // Prep Script
            Status = ScriptEngineStatus.PrepScript;
            PrepScript(script);

            Status = ScriptEngineStatus.RunningScript;
            var currentNode = script.Root.Children[0];

            ExecutionUnit.ProgramStack.Push(currentNode);
            ExecutionUnit.IndexStack.Push(1);

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
        }

        internal void PushReturnCurrentNode()
        {
            ExecutionUnit.ReturnStack.Push(ExecutionUnit.ProgramStack.Count);
        }
        
        public AdvanceType Update(AdvanceType advanceType = AdvanceType.Continue)
        {
            if (ExecutionUnit.ProgramStack.Count == 0)
            {
                Status = ScriptEngineStatus.NoScriptRunning;
                return AdvanceType.Finished;
            }

            var currentNode = ExecutionUnit.ProgramStack.Peek();
            var rv = advanceType;

            while(rv != AdvanceType.Yield &&
                  rv != AdvanceType.Finished)
            {
                switch(rv)
                {
                    case AdvanceType.Return:
                        {
                            if(ExecutionUnit.ProgramStack.Count > 0)
                            {
                                currentNode = ExecutionUnit.ProgramStack.Pop();
                                ExecutionUnit.IndexStack.Pop();
                            }
                            else
                            {
                                currentNode = null;
                            }
                            break;
                        }
                    case AdvanceType.JumpToNode:
                        {
                            // Push Call Node
                            ExecutionUnit.ProgramStack.Push(currentNode);
                            ExecutionUnit.IndexStack.Push(currentNode.Children.Count); // Should be 0

                            // TODO Implement Get by Id
                            // TODO come up with something better than the 'as' statement 
                            var name = currentNode as RNodeCallPage;
                            currentNode = m_ReferencesTable.GetPageByName(name.PageName);

                            // Push the destination Node
                            ExecutionUnit.ProgramStack.Push(currentNode);
                            ExecutionUnit.IndexStack.Push(0);
                            break;
                        }
                    case AdvanceType.ChildN:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[ExecutionUnit.ChildNRegister];
                            ExecutionUnit.ProgramStack.Push(currentNode);
                            ExecutionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.FirstChild:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[0];
                            ExecutionUnit.ProgramStack.Push(currentNode);
                            ExecutionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.SecondChild:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[1];
                            ExecutionUnit.ProgramStack.Push(currentNode);
                            ExecutionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.Next:
                        {
                            // Choose next sibling.
                            // If no siblings left, choose parents sibling.
                            currentNode = ExecutionUnit.ProgramStack.Pop();
                            var index = ExecutionUnit.IndexStack.Pop();

                            if(ExecutionUnit.ProgramStack.Count == 0)
                            {
                                rv = AdvanceType.Finished;
                                continue;
                            }

                            var parent = ExecutionUnit.ProgramStack.Peek();
                            index++;
                            if(index >= parent.Children.Count)
                            {
                                // Check the return stack if we need to enter a particular location.
                                if(ExecutionUnit.ReturnStack.Count > 0)
                                {
                                    var pindex = ExecutionUnit.ReturnStack.Pop();
                                    var indexValue = 0;
                                    while(ExecutionUnit.ProgramStack.Count > pindex)
                                    {
                                        ExecutionUnit.ProgramStack.Pop();
                                        indexValue = ExecutionUnit.IndexStack.Pop();
                                    }
                                    currentNode = ExecutionUnit.ProgramStack.Peek();

                                    rv = AdvanceType.Continue;
                                    continue;
                                }

                                rv = AdvanceType.Next;
                                continue;
                            }
                            else
                            {
                                currentNode = parent.Children[index];
                                ExecutionUnit.ProgramStack.Push(currentNode);
                                ExecutionUnit.IndexStack.Push(index);
                            }
                            break;
                        }
                    case AdvanceType.Parent:
                        {
                            if(ExecutionUnit.ProgramStack.Count > 0)
                            {
                                currentNode = ExecutionUnit.ProgramStack.Pop();
                                ExecutionUnit.IndexStack.Pop();
                            }
                            else
                            {
                                currentNode = null;
                            }
                            break;
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

        internal void MarkOnceOnlyAsVisited(Guid id)
        {
            m_OnlyIfTable.MarkVisited(id);
        }

        internal bool HasOnceOnlyBeenExecuted(Guid id)
        {
            return m_OnlyIfTable.HasOnlyIfBeenExecuted(id);
        }
    }
}
