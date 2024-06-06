using UnityEngine;

namespace Brawl.State
{
    public class FollowState : AgentState
    {
        public const float ViewRadius = 5f;
        public const float FollowingDistance = 2f;        
        private readonly Collider[] hitColliders = new Collider[5];
        private readonly Transform target;
        private readonly float maxChaseRange;

        public FollowState(AgentController agent, Transform target) : base(agent)
        {
            this.target = target;
            maxChaseRange = agent.Controller.GetAttribute("MaxChaseRange") ?? 10f;
            OnUpdateState += CheckEnemyToChase;
        }

        private static AgentState CheckEnemyToChase(AgentState currentState)
        {
            if (currentState is not FollowState followState)
            {
                Debug.LogError("CheckEnemy can only work during PatrolState.");
                return null;
            }

            int count = Physics.OverlapSphereNonAlloc(currentState.Agent.transform.position, Mathf.Min(followState.maxChaseRange, ViewRadius), followState.hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = followState.hitColliders[i];
                Controller controller = hitCollider.GetComponent<Controller>();
                if (controller != null && controller.FactionId != currentState.Agent.Controller.FactionId)
                {
                    return new MeleeChaseState(followState.Agent, controller, followState.maxChaseRange);
                }
            }
            return null;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (Vector3.Distance(target.position, Agent.Controller.Agent.destination) > FollowingDistance)
            {
                Agent.Controller.Agent.SetDestination(target.position);
            }
        }
    }
}