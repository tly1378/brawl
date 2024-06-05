namespace Brawl.State
{
    public abstract class AgentState
    {
        protected readonly AgentController agent;
        public AgentState(AgentController agent) => this.agent = agent;
        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void ExitState() { }
    }
}