using UnityEngine;

namespace Brawl.State
{
    public abstract class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        [Adjustable] private float escapeThreshold = 0.2f;
        [Adjustable] private float escapeProbability = 0.5f;
        protected Controller target;
        private float maxChaseRange;
        private Vector3 originalPosition;

        public ChaseState(AgentController agent) : base(agent)
        {
        }

        public void Set(Controller target, float maxChaseRange = float.MaxValue)
        {
            this.target = target;
            Agent.Controller.Attack.Target = target.Health;
            this.maxChaseRange = maxChaseRange;
        }

        public override void EnterState()
        {
            originalPosition = Agent.transform.position;
            hasCheckedEscape = false;
            Agent.Controller.Health.OnTakeDamage += HandleTakeDamage;
            OnUpdateState += CheckEnemyAlive;
            OnUpdateState += CheckOverRange;
        }

        public override void ExitState()
        {
            Agent.Controller.Health.OnTakeDamage -= HandleTakeDamage;
            OnUpdateState -= CheckEnemyAlive;
            OnUpdateState -= CheckOverRange;
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
                chaseState.Agent.Controller.NavAgent.SetDestination(chaseState.originalPosition);
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
                        Debug.Log(Agent.name + "选择逃跑");
                    }
                    else
                    {
                        Debug.Log(Agent.name + "选择拼命");
                    }
                    hasCheckedEscape = true;
                }
            }
        }
    }
}