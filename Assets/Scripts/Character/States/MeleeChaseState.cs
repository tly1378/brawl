using UnityEngine;

namespace Brawl.State
{
    public class MeleeChaseState : ChaseState
    {
        public MeleeChaseState(AgentController agent) : base(agent)
        {
            OnUpdateState += CheckTargetAlive;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.NavAgent.destination;
            if (Vector3.Distance(targetPosition, Agent.Controller.NavAgent.destination) > 1)
            {
                if (Vector3.Distance(targetPosition, agentPosition) > attackDistance)
                {
                    Agent.Controller.NavAgent.SetDestination(targetPosition);
                }
            }
        }
    }
}