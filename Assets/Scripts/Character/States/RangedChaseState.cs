using UnityEngine;

namespace Brawl.State
{
    public class RangedChaseState : ChaseState
    {
        private readonly float maxDistance;
        private readonly float minDistance;

        public RangedChaseState(AgentController agent) : base(agent)
        {
            maxDistance = attackDistance - 2;
            minDistance = 3;
            OnUpdateState += CheckTargetAlive;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.NavAgent.destination;
            
            // 远程：保持在适当的攻击距离
            float distanceToTarget = Vector3.Distance(targetPosition, agentPosition);

            if (distanceToTarget > maxDistance)
            {
                // 距离太远，靠近目标
                Vector3 direction = (agentPosition - targetPosition).normalized;
                Vector3 newPosition = targetPosition + direction * maxDistance;
                Agent.Controller.NavAgent.SetDestination(newPosition);
            }
            else if (distanceToTarget < minDistance)
            {
                // 距离太近，远离目标
                Vector3 direction = (agentPosition - targetPosition).normalized;
                Vector3 newPosition = targetPosition + direction * minDistance;
                Agent.Controller.NavAgent.SetDestination(newPosition);
            }            
        }
    }
}