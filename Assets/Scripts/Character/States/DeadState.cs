
namespace Brawl.State
{
    internal class DeadState : AgentState
    {
        public DeadState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            agent.Controller.Health.OnRespawn += OnRespawn;
        }

        public override void ExitState()
        {
            agent.Controller.Health.OnRespawn -= OnRespawn;
        }

        private void OnRespawn()
        {
            agent.TransitionToState(new PatrolState(agent));
        }
    }
}