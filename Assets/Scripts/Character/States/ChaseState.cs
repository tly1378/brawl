using UnityEngine;

namespace Brawl.State
{
    public class ChaseState : AgentState
    {
        private bool hasCheckedEscape;
        [SerializeField] private float escapeThreshold = 0.2f;
        [SerializeField] private float escapeProbability = 0.5f;
        private readonly Controller target;
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
            updateChecker.Add(CheckEnemyAlive);
            updateChecker.Add(CheckOverRange);
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

        // 定义近战和远程攻击的距离阈值
        float meleeAttackRange = 1.0f;
        float rangedAttackDistance = 10.0f;

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.Agent.destination;

            if (Agent.isMelee)
            {
                // 近战：如果距离大于近战攻击范围，继续接近目标
                if (Vector3.Distance(targetPosition, agentPosition) > meleeAttackRange)
                {
                    Agent.Controller.Agent.SetDestination(targetPosition);
                }
            }
            else
            {
                // 远程：保持在适当的攻击距离
                float distanceToTarget = Vector3.Distance(targetPosition, agentPosition);

                if (distanceToTarget > rangedAttackDistance)
                {
                    // 距离太远，靠近目标
                    Vector3 direction = (agentPosition - targetPosition).normalized;
                    Vector3 newPosition = targetPosition + direction * rangedAttackDistance;
                    Agent.Controller.Agent.SetDestination(newPosition);
                }
                else if (distanceToTarget < rangedAttackDistance * 0.5f)
                {
                    // 距离太近，远离目标
                    Vector3 direction = (agentPosition - targetPosition).normalized;
                    Vector3 newPosition = targetPosition + direction * rangedAttackDistance;
                    Agent.Controller.Agent.SetDestination(newPosition);
                }
            }
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