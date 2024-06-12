using UnityEngine;

namespace Brawl.State
{
    public abstract class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        [SerializeField] private float escapeThreshold = 0.2f;
        [SerializeField] private float escapeProbability = 0.5f;
        protected Controller target;
        private float maxChaseRange;
        private readonly Vector3 originalPosition;

        public ChaseState(AgentController agent) : base(agent)
        {
            escapeThreshold = agent.Controller.GetAttribute("EscapeThreshold").GetValueOrDefault(escapeThreshold);
            escapeProbability = agent.Controller.GetAttribute("EscapeProbability").GetValueOrDefault(escapeProbability);
            originalPosition = agent.transform.position;
            OnUpdateState += CheckEnemyAlive;
            OnUpdateState += CheckOverRange;
        }

        public void Set(Controller target, float maxChaseRange = float.MaxValue)
        {
            this.target = target;
            Agent.Controller.Attack.Target = target.Health;
            this.maxChaseRange = maxChaseRange;
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

        private static string CheckEnemyAlive(AgentState currentState)
        {
            if (currentState is not ChaseState chaseState)
            {
                Debug.LogError("CheckEnemyAlive can only work during ChaseState.");
                return null;
            }

            if (chaseState.target == null || !chaseState.target.Health.IsAlive)
            {
                return nameof(PatrolState);
            }
            return null;
        }

        private static string CheckOverRange(AgentState currentState)
        {
            if (currentState is not ChaseState chaseState)
            {
                Debug.LogError("CheckOverRange can only work during ChaseState.");
                return null;
            }

            if (Vector3.Distance(chaseState.originalPosition, chaseState.Agent.transform.position) > chaseState.maxChaseRange)
            {
                chaseState.Agent.Controller.Agent.SetDestination(chaseState.originalPosition);
                return nameof(PatrolState);
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
                        Agent.TransitionToState(nameof(HealState));
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