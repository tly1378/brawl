using UnityEngine;

public class HealState : AgentState
{
    private const float HealDistance = 1f;

    public HealState(AgentController agent) : base(agent)
    {
    }

    public override void EnterState()
    {
        agent.Agent.SetDestination(agent.HealPoint.position);
    }

    public override void UpdateState()
    {
        if (!agent.IsAlive)
        {
            agent.TransitionToState(new DeadState(agent));
            return;
        }

        if (Vector3.Distance(agent.transform.position, agent.HealPoint.position) < HealDistance)
        {
            if (agent.Health.CurrentHealth >= agent.Health.MaxHealth)
            {
                agent.TransitionToState(new PatrolState(agent));
            }
        }
    }
}
