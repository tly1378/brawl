public class AgentState
{
    public virtual void EnterState(AgentController agent) { }
    public virtual void UpdateState(AgentController agent) { }
    public virtual void ExitState(AgentController agent) { }
}
