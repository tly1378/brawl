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

        private string CheckHPFull()
        {
            var distance = Vector3.Distance(Agent.transform.position, Agent.HealPoint.position);
            if (distance < HealDistance && Agent.Controller.Health.CurrentHealth >= Agent.Controller.Health.MaxHealth)
            {
                return nameof(PatrolState);
            }
            return null;
        }
    }
}