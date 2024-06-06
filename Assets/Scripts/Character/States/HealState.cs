using UnityEngine;

namespace Brawl.State
{
    public class HealState : AgentState
    {
        private const float HealDistance = 1f;

        public HealState(AgentController agent) : base(agent)
        {
            updateChecker.Add(CheckHPFull);
        }

        public override void EnterState()
        {
            Agent.Controller.Agent.SetDestination(Agent.HealPoint.position);
        }

        private static AgentState CheckHPFull(AgentState currentState)
        {
            var distance = Vector3.Distance(currentState.Agent.transform.position, currentState.Agent.HealPoint.position);
            if (distance < HealDistance && currentState.Agent.Controller.Health.CurrentHealth >= currentState.Agent.Controller.Health.MaxHealth)
            {
                return new PatrolState(currentState.Agent);
            }
            return null;
        }
    }
}