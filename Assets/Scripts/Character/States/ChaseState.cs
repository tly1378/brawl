using UnityEngine;

namespace Brawl.State
{
    public class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        private float escapeThreshold = 0.2f;
        private float escapeProbability = 0.5f;
        protected Controller target;

        public ChaseState(AgentController agent, Controller target) : base(agent)
        {
            escapeThreshold = agent.GetAttribute("EscapeThreshold").GetValueOrDefault(escapeThreshold);
            escapeProbability = agent.GetAttribute("EscapeProbability").GetValueOrDefault(escapeProbability);
            this.target = target;
            agent.Melee.Target = target.Health;
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
            if (target == null || !target.Health.IsAlive)
            {
                agent.TransitionToState(new PatrolState(agent));
                return;
            }

            if (Vector3.Distance(target.transform.position, agent.Agent.destination) > 1)
            {
                agent.Agent.SetDestination(target.transform.position);
            }
        }

        private void HandleTakeDamage()
        {
            if (agent.Health.CurrentHealth <= escapeThreshold * agent.Health.MaxHealth)
            {
                if (!hasCheckedEscape)
                {
                    if (Random.value < escapeProbability)
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