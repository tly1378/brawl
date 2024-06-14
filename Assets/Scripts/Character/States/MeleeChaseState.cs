﻿using UnityEngine;

namespace Brawl.State
{
    public class MeleeChaseState : ChaseState
    {
        private readonly float meleeAttackRange = 1.0f;

        public MeleeChaseState(AgentController agent) : base(agent)
        {
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.NavAgent.destination;
            if (Vector3.Distance(targetPosition, agentPosition) > meleeAttackRange)
            {
                Agent.Controller.NavAgent.SetDestination(targetPosition);
            }
        }
    }
}