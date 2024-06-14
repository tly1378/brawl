using UnityEngine;

namespace Brawl.State
{
    public class FollowState : AgentState
    {
        public const float ViewRadius = 5f;
        public const float FollowingDistance = 2f;        
        private readonly Collider[] hitColliders = new Collider[5];
        [Adjustable] private readonly float maxChaseRange = 10f;
        private Transform target;

        public FollowState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            OnUpdateState += CheckEnemyToChase;
        }

        public override void ExitState()
        {
            base.ExitState();
            OnUpdateState -= CheckEnemyToChase;
        }

        public void Set(Transform target)
        {
            this.target = target;
        }

        private static string CheckEnemyToChase(AgentState currentState)
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
                    (currentState.Agent.stateDict[nameof(ChaseState)] as ChaseState).Set(controller, followState.maxChaseRange);
                    return nameof(ChaseState);
                }
            }
            return null;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (Vector3.Distance(target.position, Agent.Controller.NavAgent.destination) > FollowingDistance)
            {
                Agent.Controller.NavAgent.SetDestination(target.position);
            }
        }
    }
}