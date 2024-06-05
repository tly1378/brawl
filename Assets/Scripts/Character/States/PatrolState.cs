using UnityEngine;
using UnityEngine.AI;

namespace Brawl.State
{
    public class PatrolState : AgentState
    {
        public const float ViewRadius = 15f;
        private const float WanderInterval = 5f;
        private float wanderTimer;
        private Vector3 originalPosition;
        private readonly Collider[] hitColliders = new Collider[5];
        private float wanderRadius;
        private float maxChaseRange;

        public PatrolState(AgentController agent) : base(agent)
        {
            originalPosition = agent.transform.position;
            wanderRadius = agent.Controller.GetAttribute("WanderRadius") ?? 10f;
            maxChaseRange = agent.Controller.GetAttribute("MaxChaseRange") ?? 10f;
        }

        public override void EnterState()
        {
            wanderTimer = WanderInterval;
            agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        public override void UpdateState()
        {
            if (agent.Controller.Health.CurrentHealth < agent.Controller.Health.MaxHealth)
            {
                agent.TransitionToState(new HealState(agent));
                return;
            }

            int count = Physics.OverlapSphereNonAlloc(agent.transform.position, Mathf.Min(maxChaseRange, ViewRadius), hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = hitColliders[i];
                Controller controller = hitCollider.GetComponent<Controller>();
                if (controller != null && controller.FactionId != agent.Controller.FactionId)
                {
                    agent.TransitionToState(new ChaseState(agent, controller, maxChaseRange));
                    return;
                }
            }

            if(wanderRadius > 0)
            {
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= WanderInterval)
                {
                    if (Vector3.Distance(agent.Controller.Agent.destination, agent.transform.position) < 1f)
                    {
                        Vector3 newPos = RandomNavSphere(originalPosition, wanderRadius);
                        agent.Controller.Agent.SetDestination(newPos);
                        wanderTimer = 0;
                    }
                }
            }
        }

        public override void ExitState()
        {
            agent.Controller.OnAttributeChange -= HandleAttributeChange;
        }

        private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask = -1)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
            return navHit.position;
        }

        private void HandleAttributeChange(string name, float value, float? origin)
        {
            if (name == "WanderRadius")
            {
                wanderRadius = value;
            }
            else if (name == "MaxChaseRange")
            {
                maxChaseRange = value;
            }
        }
    }
}