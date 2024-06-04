namespace Brawl
{
    public class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        private float escapeThreshold = 0.2f;
        private float escapeProbability = 0.5f;

        public ChaseState(AgentController agent) : base(agent)
        {
            escapeThreshold = agent.GetAttribute("EscapeThreshold").GetValueOrDefault(escapeThreshold);
            escapeProbability = agent.GetAttribute("EscapeProbability").GetValueOrDefault(escapeProbability);
        }

        public override void EnterState()
        {
            agent.Health.OnTakeDamage += HandleTakeDamage;
            agent.OnAttributeChange += HandleAttributeChange;
        }

        public override void ExitState()
        {
            agent.Health.OnTakeDamage -= HandleTakeDamage;
            agent.OnAttributeChange -= HandleAttributeChange;
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
            if (agent.Health.CurrentHealth <= escapeThreshold * agent.Health.MaxHealth)
            {
                if (!hasCheckedEscape)
                {
                    if (UnityEngine.Random.value < escapeProbability)
                    {
                        agent.TransitionToState(new HealState(agent));
                    }
                    hasCheckedEscape = true;
                }
            }
        }

        private void HandleAttributeChange(string name, float value, float? origin)
        {
            if (name == "EscapeThreshold")
            {
                escapeThreshold = value;
            }
            else if (name == "EscapeProbability")
            {
                escapeProbability = value;
            }
        }
    }
}