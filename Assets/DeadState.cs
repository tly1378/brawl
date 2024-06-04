
namespace Brawl
{
    internal class DeadState : AgentState
    {
        public DeadState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            agent.Health.OnRespawn += OnRespawn;
        }

        public override void ExitState()
        {
            agent.Health.OnRespawn -= OnRespawn;
        }

        private void OnRespawn()
        {
            agent.TransitionToState(new PatrolState(agent));
        }
    }
}