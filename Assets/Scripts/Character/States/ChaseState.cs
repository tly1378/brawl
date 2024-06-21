using System;
using UnityEngine;

namespace Brawl.State
{
    public abstract class ChaseState : AgentState
    {
        protected Controller target;
        protected readonly float attackDistance;
        private bool hasCheckedEscape;
        private float maxChaseRange;
        private Vector3 originalPosition;
        private float escapeThreshold = 0.2f;
        private float escapeHp = 20;

        public float EscapeThreshold
        {
            get { return escapeThreshold; }
            set
            {
                escapeHp = value * Agent.Controller.Health.MaxHealth;
                escapeThreshold = value;
            }
        }
        public float EscapeProbability { get; set; } = 0.5f;

        public ChaseState(AgentController agent) : base(agent)
        {
            attackDistance = agent.Controller.Attack.attackRange;
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
            OnUpdateState += CheckOverRange;
        }

        public override void ExitState()
        {
            Agent.Controller.Health.OnTakeDamage -= HandleTakeDamage;
            OnUpdateState -= CheckOverRange;
        }

        protected string CheckTargetAlive()
        {
            if (target == null || !target.Health.IsAlive)
            {
                return nameof(PatrolState);
            }
            return null;
        }

        private string CheckOverRange()
        {
            if (Vector3.Distance(originalPosition, Agent.transform.position) > maxChaseRange)
            {
                Agent.Controller.NavAgent.SetDestination(originalPosition);
                return nameof(PatrolState);
            }
            return null;
        }

        private void HandleTakeDamage()
        {
            if (Agent.Controller.Health.CurrentHealth <= escapeHp)
            {
                if (!hasCheckedEscape)
                {
                    if (UnityEngine.Random.value < EscapeProbability)
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