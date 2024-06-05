using UnityEngine;

namespace Brawl.State
{
    public class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        private float escapeThreshold = 0.2f;
        private float escapeProbability = 0.5f;
        private readonly Controller target;
        private readonly float range;
        private readonly Vector3 originalPosition;

        public ChaseState(AgentController agent, Controller target, float range = float.MaxValue) : base(agent)
        {
            escapeThreshold = agent.Controller.GetAttribute("EscapeThreshold").GetValueOrDefault(escapeThreshold);
            escapeProbability = agent.Controller.GetAttribute("EscapeProbability").GetValueOrDefault(escapeProbability);
            this.range = range;
            this.target = target;
            agent.Controller.Melee.Target = target.Health;
            originalPosition = agent.transform.position;
        }

        public override void EnterState()
        {
            agent.Controller.Health.OnTakeDamage += HandleTakeDamage;
            agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        public override void ExitState()
        {
            agent.Controller.Health.OnTakeDamage -= HandleTakeDamage;
            agent.Controller.OnAttributeChange -= HandleAttributeChange;
        }

        public override void UpdateState()
        {
            if (target == null || !target.Health.IsAlive)
            {
                agent.TransitionToState(new PatrolState(agent));
                return;
            }

            if (Vector3.Distance(originalPosition, agent.transform.position) > range)
            {
                agent.Controller.Agent.SetDestination(originalPosition);
                agent.TransitionToState(new PatrolState(agent));
                return;
            }

            if (Vector3.Distance(target.transform.position, agent.Controller.Agent.destination) > 1)
            {
                agent.Controller.Agent.SetDestination(target.transform.position);
            }
        }

        private void HandleTakeDamage()
        {
            if (agent.Controller.Health.CurrentHealth <= escapeThreshold * agent.Controller.Health.MaxHealth)
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