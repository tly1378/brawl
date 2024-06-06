using UnityEngine;

namespace Brawl.State
{
    public abstract class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        [SerializeField] private float escapeThreshold = 0.2f;
        [SerializeField] private float escapeProbability = 0.5f;
        protected readonly Controller target;
        private readonly float range;
        private readonly Vector3 originalPosition;

        public ChaseState(AgentController agent, Controller target, float range = float.MaxValue) : base(agent)
        {
            escapeThreshold = agent.Controller.GetAttribute("EscapeThreshold").GetValueOrDefault(escapeThreshold);
            escapeProbability = agent.Controller.GetAttribute("EscapeProbability").GetValueOrDefault(escapeProbability);
            this.range = range;
            this.target = target;
            agent.Controller.Attack.Target = target.Health;
            originalPosition = agent.transform.position;
            OnUpdateState += CheckEnemyAlive;
            OnUpdateState += CheckOverRange;
        }

        public override void EnterState()
        {
            Agent.Controller.Health.OnTakeDamage += HandleTakeDamage;
            Agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        public override void ExitState()
        {
            Agent.Controller.Health.OnTakeDamage -= HandleTakeDamage;
            Agent.Controller.OnAttributeChange -= HandleAttributeChange;
        }

        private static AgentState CheckEnemyAlive(AgentState currentState)
        {
            if (currentState is not ChaseState chaseState)
            {
                Debug.LogError("CheckEnemyAlive can only work during ChaseState.");
                return null;
            }

            if (chaseState.target == null || !chaseState.target.Health.IsAlive)
            {
                return new PatrolState(chaseState.Agent);
            }
            return null;
        }

        private static AgentState CheckOverRange(AgentState currentState)
        {
            if (currentState is not ChaseState chaseState)
            {
                Debug.LogError("CheckOverRange can only work during ChaseState.");
                return null;
            }

            if (Vector3.Distance(chaseState.originalPosition, chaseState.Agent.transform.position) > chaseState.range)
            {
                chaseState.Agent.Controller.Agent.SetDestination(chaseState.originalPosition);
                return new PatrolState(chaseState.Agent);
            }
            return null;
        }

        private void HandleTakeDamage()
        {
            if (Agent.Controller.Health.CurrentHealth <= escapeThreshold * Agent.Controller.Health.MaxHealth)
            {
                if (!hasCheckedEscape)
                {
                    if (Random.value < escapeProbability)
                    {
                        Agent.TransitionToState(new HealState(Agent));
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