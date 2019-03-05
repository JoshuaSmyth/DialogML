﻿using System;
using System.Collections.Generic;

namespace DialogML.DNodes
{
    enum RNodeSelectState
    {
        OnEnter = 0, // Or OnReturn
        AwaitingResult = 1,
        SelectedOption = 2,
        OnReEnter = 3,
        OnReEnterSelectedExitNode = 4
    }

    public class RNodeSelect : RNode
    {
        // May need an OnReset() method
        RNodeSelectState State;
        Int32 SelectedIndex = 0;

        Option SelectedOption;

        public bool RemoveOnSelect;
        public RNodeSelect(Guid id, bool removeOnSelect)
        {
            Id = id;
            RemoveOnSelect = removeOnSelect;
        }
        
        public override AdvanceType Execute(ScriptApi api)
        {
            if (State == RNodeSelectState.OnReEnterSelectedExitNode)
            {
                return AdvanceType.Next; // Probably only works if last.
            }

            if (State == RNodeSelectState.OnReEnter)
            {
                State = RNodeSelectState.OnEnter;
            }

            if (State== RNodeSelectState.OnEnter)
            {
                Console.WriteLine();
                api.Trace("Select Node");

                var options = new List<Option>();
                var selection = new OptionSelection(options);
                var index = 1;
                foreach(var c in this.Children)
                {
                    // TODO Evaluate if option should be shown
                    if (c is RNodeOption)
                    {
                        var o = c as RNodeOption;
                        options.Add(new Option() { Id = o.Id, IsExit = false, ChildIndex=index });
                    }
                    if (c is RNodeOptionExit)
                    {
                        var o = c as RNodeOptionExit;
                        options.Add(new Option() { Id = o.Id, IsExit = true, ChildIndex=index });
                    }
                    index++;
                }
                // TODO If there is no option exit generate one
                //  - Unless flag for auto-generate-exit-option='false'

                // TODO If list is empty return next
                
                Console.WriteLine();
                
                api.OnSelectOption(options, (o) =>
                {
                    // TODO On select increment the selectedOption table
                    SelectedOption = o;
                    
                    api.PushReturnCurrentNode();

                    SelectedIndex = o.ChildIndex;
                    State = RNodeSelectState.SelectedOption;
                    if(o.IsExit)
                    {
                        State = RNodeSelectState.OnReEnterSelectedExitNode;
                    }
                });

                return AdvanceType.Yield;
            }
            else if (State == RNodeSelectState.AwaitingResult)
            {
                return AdvanceType.Yield;
            }
            else if (State == RNodeSelectState.SelectedOption)
            {
                // If this is an exit node then it's not child N but parent
                // Unless we want to explore the nodes first
                State = RNodeSelectState.OnReEnter;
                api.SetChildNRegister(SelectedIndex);
                return AdvanceType.ChildN;
            }

            return AdvanceType.Yield;
        }

        public override void Prep()
        {
            State = RNodeSelectState.OnEnter;
        }
    }
}
