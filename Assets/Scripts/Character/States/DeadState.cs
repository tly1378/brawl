
namespace Brawl.State
{
    internal class DeadState : AgentState
    {
        public DeadState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            Agent.Controller.Health.OnRespawn += OnRespawn;
        }

        public override void ExitState()
        {
            Agent.Controller.Health.OnRespawn -= OnRespawn;
        }

        private void OnRespawn()
        {
            Agent.TransitionToState(StateEnum.Patrol);
        }
    }
}