public class ChaseState : AgentState
{
    public const float HealthThreshold = 0.2f;
    public const float EscapeProbability = 0.5f;
    private bool hasCheckedEscape;
    private AgentController agent;

    public override void EnterState(AgentController agent)
    {
        this.agent = agent;
        agent.Health.OnTakeDamage += HandleTakeDamage;
    }

    public override void UpdateState(AgentController agent)
    {
        if (!agent.IsAlive)
        {
            agent.TransitionToState(new DeadState());
            return;
        }

        if (agent.Target != null)
        {
            agent.Agent.SetDestination(agent.Target.position);

            Health targetHealth = agent.Target.GetComponent<Health>();
            if (targetHealth == null || targetHealth.CurrentHealth <= 0)
            {
                agent.Target = null;
                agent.TransitionToState(new PatrolState());
            }
        }
        else
        {
            agent.TransitionToState(new PatrolState());
        }
    }

    private void HandleTakeDamage()
    {
        if (!hasCheckedEscape && agent.Health.CurrentHealth <= HealthThreshold * agent.Health.MaxHealth)
        {
            if (UnityEngine.Random.value < EscapeProbability)
            {
                agent.TransitionToState(new HealState());
            }
            hasCheckedEscape = true;
        }
    }
}
