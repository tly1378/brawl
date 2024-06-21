using System;
using System.Collections.Generic;
using Brawl.State;
using UnityEngine;

namespace Brawl
{
    internal class HealChaseState : ChaseState
    {
        private float healDistance;
        private readonly List<Controller> teammates = new();

        public HealChaseState(AgentController agent) : base(agent)
        {
            foreach (Controller controller in CharacterManager.Instance.Controllers)
            {
                if (controller.FactionId == agent.Controller.FactionId)
                {
                    teammates.Add(controller);
                }
            }

            OnUpdateState += CheckPartnerFullHealth;
            OnUpdateState += CheckTargetAlive;
            healDistance = attackDistance * 0.9f;
        }

        protected new string CheckTargetAlive()
        {
            if (target == null || !target.Health.IsAlive)
            {
                foreach (Controller teammate in teammates)
                {
                    if (teammate.Health.IsAlive && teammate.Health.CurrentHealth < teammate.Health.MaxHealth)
                    {
                        var state = Agent.stateDict[nameof(ChaseState)] as HealChaseState;
                        state.Set(teammate);
                        return nameof(ChaseState);
                    }
                }
            }
            return null;
        }

        private string CheckPartnerFullHealth()
        {
            if(target.Health.CurrentHealth >= target.Health.MaxHealth)
            {
                return nameof(PatrolState);
            }
            return null;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // 获取目标和代理的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 agentPosition = Agent.Controller.NavAgent.destination;
            if (Vector3.Distance(targetPosition, Agent.Controller.NavAgent.destination) > 0.1f)
            {
                if (Vector3.Distance(targetPosition, agentPosition) > healDistance)
                {
                    Agent.Controller.NavAgent.SetDestination(targetPosition);
                }
            }
        }
    }
}