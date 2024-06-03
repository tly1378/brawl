using UnityEngine;

public class HealState : AgentState
{
    public override void EnterState(AgentController agent)
    {
        agent.Agent.SetDestination(agent.HealPoint.position);
    }

    public override void UpdateState(AgentController agent)
    {
        if (!agent.IsAlive)
        {
            agent.TransitionToState(new DeadState());
            return;
        }

        if (Vector3.Distance(agent.transform.position, agent.HealPoint.position) < AgentController.HealDistance)
        {
            if (agent.Health.CurrentHealth >= agent.Health.MaxHealth)
            {
                agent.TransitionToState(new PatrolState());
            }
        }
    }
}
