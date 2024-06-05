using UnityEngine;

namespace Brawl.State
{
    public class FollowState : AgentState
    {
        public const float ViewRadius = 5f;
        public const float FollowingDistance = 2f;        
        private readonly Collider[] hitColliders = new Collider[5];
        private readonly Transform target;

        public FollowState(AgentController agent, Transform target) : base(agent)
        {
            this.target = target;
        }

        public override void UpdateState()
        {
            if (Vector3.Distance(target.position, agent.Controller.Agent.destination) > FollowingDistance)
            {
                agent.Controller.Agent.SetDestination(target.position);
            }

            int count = Physics.OverlapSphereNonAlloc(agent.transform.position, ViewRadius, hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = hitColliders[i];
                Controller target = hitCollider.GetComponent<Controller>();
                if (target != null && target.FactionId != agent.Controller.FactionId)
                {
                    agent.TransitionToState(new ChaseState(agent, target));
                    break;
                }
            }
        }
    }
}