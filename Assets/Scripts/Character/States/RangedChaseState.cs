using UnityEngine;

namespace Brawl.State
{
    public class RangedChaseState : ChaseState
    {
        private readonly float rangedAttackDistance = 10.0f;

        public RangedChaseState(AgentController agent) : base(agent)
        {
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.Agent.destination;
            
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
}