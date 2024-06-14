using UnityEngine;

namespace Brawl.State
{
    public class HealState : AgentState
    {
        private const float HealDistance = 1f;

        public HealState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            OnUpdateState += CheckHPFull;
            Agent.Controller.NavAgent.SetDestination(Agent.HealPoint.position);
        }

        public override void ExitState()
        {
            base.ExitState();
            OnUpdateState -= CheckHPFull;
        }

        private static string CheckHPFull(AgentState currentState)
        {
            var distance = Vector3.Distance(currentState.Agent.transform.position, currentState.Agent.HealPoint.position);
            if (distance < HealDistance && currentState.Agent.Controller.Health.CurrentHealth >= currentState.Agent.Controller.Health.MaxHealth)
            {
                return nameof(PatrolState);
            }
            return null;
        }
    }
}