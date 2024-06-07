using UnityEngine;

namespace Brawl.State
{
    public class FollowState : AgentState
    {
        public const float ViewRadius = 5f;
        public const float FollowingDistance = 2f;        
        private readonly Collider[] hitColliders = new Collider[5];
        private readonly float maxChaseRange;
        private Transform target;

        public FollowState(AgentController agent) : base(agent)
        {
            maxChaseRange = agent.Controller.GetAttribute("MaxChaseRange") ?? 10f;
            OnUpdateState += CheckEnemyToChase;
        }

        public void Set(Transform target)
        {
            this.target = target;
        }

        private static StateEnum? CheckEnemyToChase(AgentState currentState)
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
                    (currentState.Agent.stateDict[StateEnum.Chase] as ChaseState).Set(controller, followState.maxChaseRange);
                    return StateEnum.Chase;
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