using System;
using System.Collections.Generic;

namespace Brawl.State
{
    public abstract class AgentState
    {
        public AgentController Agent { get; }

        public AgentState(AgentController agent) => Agent = agent;

        public delegate AgentState StateChecker(AgentState state);

        public readonly List<StateChecker> updateChecker = new();

        public virtual void EnterState() { }

        public virtual void UpdateState() 
        {
            foreach (var checker in updateChecker)
            {
                Agent.TransitionToState(checker(this));
            }
        }

        public virtual void ExitState() { }
    }
}