internal class DeadState : AgentState
{
    private AgentController agent;

    public override void EnterState(AgentController agent)
    {
        this.agent = agent;
        agent.Health.OnRespawn += OnRespawn;
    }

    private void OnRespawn()
    {
        agent.TransitionToState(new PatrolState());
    }
}