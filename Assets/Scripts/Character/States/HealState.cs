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
            agent.Agent.SetDestination(agent.HealPoint.position);
        }

        public override void UpdateState()
        {
            if (Vector3.Distance(agent.transform.position, agent.HealPoint.position) < HealDistance && agent.Health.CurrentHealth >= agent.Health.MaxHealth)
            {
                agent.TransitionToState(new PatrolState(agent));
                return;
            }
        }
    }
}