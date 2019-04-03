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
        CallScript = 19,
        Vars = 20,
        DeclareGlobalVar = 21,
        Set = 22,
        Noop = 254, 
        Custom = 255,
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
        CallScript,
        Return
    }

    public enum ScriptEngineStatus
    {
        NoScriptRunning,
        RunningScript,
        PrepScript
    }

    public class ExecutionUnit
    {
        public Guid CallPageId { get; internal set; }

        public Guid JumpRegister;
        public Int32 ChildNRegister;

        public Stack<RNode> ProgramStack = new Stack<RNode>();    // This is how we navigate the tree
        public Stack<int> IndexStack = new Stack<int>();          // Keep a record of what child index we are at.
        public Stack<int> ReturnStack = new Stack<int>();         // index into the program stack to change the default behaviour
                                                                  // of returning to the parent when reaching the end of the children.
        // TODO Replace these two as a guid to the node
       // public string CallPageRegister;
       // public string CallScriptRegister;
    }

    public class ScriptEngine
    {
        public ScriptEngineStatus Status;
        
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

        // TODO Pass in execution unit
        internal void SetChildNRegister(int n)
        {
            ExecutionUnit.ChildNRegister = n;
        }

        public void PrepScriptRecursive(RNode node)
        {
            // TODO This info should be on a execution unit callstatestack
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

        public void CreateParallelUnit(RNode root)
        {
            var unit = new ExecutionUnit();

            unit.ProgramStack.Push(root);
            unit.IndexStack.Push(1);

            m_ParallelUnits.Add(unit);
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
            
            return AdvanceType.Continue;
         }

        internal void PushReturnParentNode()
        {
            throw new Exception("Don't call this method TODO Remove");
        }

        internal void PushReturnCurrentNode()
        {
            ExecutionUnit.ReturnStack.Push(ExecutionUnit.ProgramStack.Count);
        }

        private List<ExecutionUnit> m_ParallelUnits = new List<ExecutionUnit>();

        public AdvanceType Update(AdvanceType advanceType = AdvanceType.Continue)
        {
            // Update each concurrent execution unit until all are resolved
            if(m_ParallelUnits.Count > 0)
            {
                for(int i = 0; i < m_ParallelUnits.Count; i++)
                {
                    var result = Update(m_ParallelUnits[i]);
                    if(result == AdvanceType.Finished)
                    {
                        m_ParallelUnits.RemoveAt(i);
                        i--;
                    }
                }
            }
            //else
            {

                return Update(ExecutionUnit, advanceType);
            }
        }

        public AdvanceType Update(ExecutionUnit executionUnit, AdvanceType advanceType = AdvanceType.Continue)
        {
            if (executionUnit.ProgramStack.Count == 0)
            {
                Status = ScriptEngineStatus.NoScriptRunning;
                return AdvanceType.Finished;
            }

            var currentNode = executionUnit.ProgramStack.Peek();
            var rv = advanceType;

            while(rv != AdvanceType.Yield &&
                  rv != AdvanceType.Finished)
            {
                switch(rv)
                {
                    case AdvanceType.Return:
                        {
                            if(executionUnit.ProgramStack.Count > 0)
                            {
                                currentNode = executionUnit.ProgramStack.Pop();
                                executionUnit.IndexStack.Pop();
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
                            executionUnit.ProgramStack.Push(currentNode);
                            executionUnit.IndexStack.Push(currentNode.Children.Count); // Should be 0

                            // TODO Implement Get by Id
                            //var name = executionUnit.CallPageRegister;
                            //currentNode = m_ReferencesTable.GetPageByName(name);

                            var id = executionUnit.CallPageId;
                            currentNode = m_ReferencesTable.GetPageById(id);
                            
                            if (currentNode == null)
                            {
                                throw new Exception("Unknown page");
                            }

                            // Prep the node for this call
                            PrepScriptRecursive(currentNode);

                            // Push the destination Node
                            executionUnit.ProgramStack.Push(currentNode);
                            executionUnit.IndexStack.Push(0);
                            break;
                        }
                    case AdvanceType.ChildN:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[executionUnit.ChildNRegister];

                            // Fixes an issue with re-entry of state
                            // When looping back to a SelectNode...
                            // Is this the best solution?
                            PrepScriptRecursive(currentNode);

                            executionUnit.ProgramStack.Push(currentNode);
                            executionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.FirstChild:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[0];
                            executionUnit.ProgramStack.Push(currentNode);
                            executionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.SecondChild:
                        {
                            int pushValue = 0;
                            currentNode = currentNode.Children[1];
                            executionUnit.ProgramStack.Push(currentNode);
                            executionUnit.IndexStack.Push(pushValue);
                            break;
                        }
                    case AdvanceType.Next:
                        {
                            // Choose next sibling.
                            // If no siblings left, choose parents sibling.
                            currentNode = executionUnit.ProgramStack.Pop();
                            var index = executionUnit.IndexStack.Pop();

                            if(executionUnit.ProgramStack.Count == 0)
                            {
                                rv = AdvanceType.Finished;
                                continue;
                            }

                            var parent = executionUnit.ProgramStack.Peek();
                            index++;
                            if(index >= parent.Children.Count)
                            {
                                // Check the return stack if we need to enter a particular location.
                                if(executionUnit.ReturnStack.Count > 0)
                                {
                                    var pindex = executionUnit.ReturnStack.Pop();
                                    var indexValue = 0;
                                    while(executionUnit.ProgramStack.Count > pindex)
                                    {
                                        executionUnit.ProgramStack.Pop();
                                        indexValue = executionUnit.IndexStack.Pop();
                                    }
                                    currentNode = executionUnit.ProgramStack.Peek();

                                    rv = AdvanceType.Continue;
                                    continue;
                                }

                                rv = AdvanceType.Next;
                                continue;
                            }
                            else
                            {
                                currentNode = parent.Children[index];
                                executionUnit.ProgramStack.Push(currentNode);
                                executionUnit.IndexStack.Push(index);
                            }
                            break;
                        }
                    case AdvanceType.Parent:
                        {
                            if(executionUnit.ProgramStack.Count > 0)
                            {
                                currentNode = executionUnit.ProgramStack.Pop();
                                executionUnit.IndexStack.Pop();
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
                    rv = currentNode.Execute(m_ScriptApi, executionUnit);
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

        internal int CountParallelNodes()
        {
            return m_ParallelUnits.Count;
            //throw new NotImplementedException();
        }
    }
}
