using UnityEngine;
using UnityEngine.AI;

namespace Brawl.State
{
    public class PatrolState : AgentState
    {
        public const float ViewRadius = 15f;
        private const float WanderRadius = 10f;
        private const float WanderInterval = 5f;
        private float wanderTimer;
        private readonly Collider[] hitColliders = new Collider[5];

        public PatrolState(AgentController agent) : base(agent)
        {
        }

        public override void EnterState()
        {
            wanderTimer = WanderInterval;
        }

        public override void UpdateState()
        {
            if (agent.Health.CurrentHealth < agent.Health.MaxHealth)
            {
                agent.TransitionToState(new HealState(agent));
                return;
            }

            wanderTimer += Time.deltaTime;

            if (wanderTimer >= WanderInterval)
            {
                Vector3 newPos = RandomNavSphere(agent.transform.position, WanderRadius, -1);
                agent.Agent.SetDestination(newPos);
                wanderTimer = 0;
            }

            int count = Physics.OverlapSphereNonAlloc(agent.transform.position, ViewRadius, hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = hitColliders[i];
                Controller controller = hitCollider.GetComponent<Controller>();
                if (controller != null && controller.factionId != agent.factionId)
                {
                    agent.TransitionToState(new ChaseState(agent, controller));
                    break;
                }
            }
        }

        private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
            return navHit.position;
        }
    }
}