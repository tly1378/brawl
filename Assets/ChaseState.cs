public class ChaseState : AgentState
{
    private bool hasCheckedEscape;
    private readonly float escapeThreshold;
    private readonly float escapeProbability;

    public ChaseState(AgentController agent) : base(agent)
    {
        escapeThreshold = agent.GetAttribute("EscapeThreshold").GetValueOrDefault(0.2f);
        escapeProbability = agent.GetAttribute("EscapeProbability").GetValueOrDefault(0.5f);
    }

    public override void EnterState()
    {
        agent.Health.OnTakeDamage += HandleTakeDamage;
    }

    public override void UpdateState()
    {
        if (!agent.IsAlive)
        {
            agent.TransitionToState(new DeadState(agent));
            return;
        }

        if (agent.Target != null)
        {
            agent.Agent.SetDestination(agent.Target.position);

            Health targetHealth = agent.Target.GetComponent<Health>();
            if (targetHealth == null || targetHealth.CurrentHealth <= 0)
            {
                agent.Target = null;
                agent.TransitionToState(new PatrolState(agent));
            }
        }
        else
        {
            agent.TransitionToState(new PatrolState(agent));
        }
    }

    private void HandleTakeDamage()
    {
        if (!hasCheckedEscape && agent.Health.CurrentHealth <= escapeThreshold * agent.Health.MaxHealth)
        {
            if (UnityEngine.Random.value < escapeProbability)
            {
                agent.TransitionToState(new HealState(agent));
            }
            hasCheckedEscape = true;
        }
    }
}
