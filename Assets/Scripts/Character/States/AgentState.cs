namespace Brawl.State
{
    public abstract class AgentState
    {
        public AgentController Agent { get; }

        public AgentState(AgentController agent) => Agent = agent;

        public delegate string StateChecker(AgentState state);

        public event StateChecker OnUpdateState;

        public virtual void EnterState() { }

        public virtual void UpdateState()
        {
            if (OnUpdateState != null)
            {
                var invocationList = OnUpdateState.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    var method = (StateChecker)invocationList[i];
                    var result = method.Invoke(this);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Agent.TransitionToState(result);
                        return;
                    }
                }
            }
        }

        public virtual void ExitState() { }
    }
}