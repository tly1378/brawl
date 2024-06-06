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
            maxChaseRange = agent.Controller.GetAttribute("MaxChaseRange") ?? 15f;

            updateChecker.Add(CheckHPToHeal);
            updateChecker.Add(CheckEnemyToChase);
        }

        public override void EnterState()
        {
            wanderTimer = WanderInterval;
            Agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        private static AgentState CheckHPToHeal(AgentState currentState)
        {
            if (currentState.Agent.Controller.Health.CurrentHealth < currentState.Agent.Controller.Health.MaxHealth)
            {
                return new HealState(currentState.Agent);
            }
            return null;
        }

        private static AgentState CheckEnemyToChase(AgentState currentState)
        {
            if (currentState is not PatrolState patrolState)
            {
                Debug.LogError("CheckEnemy can only work during PatrolState.");
                return null;
            }

            int count = Physics.OverlapSphereNonAlloc(currentState.Agent.transform.position, Mathf.Min(patrolState.maxChaseRange, ViewRadius), patrolState.hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = patrolState.hitColliders[i];
                Controller controller = hitCollider.GetComponent<Controller>();
                if (controller != null && controller.FactionId != currentState.Agent.Controller.FactionId)
                {
                    return new ChaseState(patrolState.Agent, controller, patrolState.maxChaseRange);
                }
            }
            return null;
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if(wanderRadius > 0)
            {
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= WanderInterval)
                {
                    if (Vector3.Distance(Agent.Controller.Agent.destination, Agent.transform.position) < 1f)
                    {
                        Vector3 newPos = RandomNavSphere(originalPosition, wanderRadius);
                        Agent.Controller.Agent.SetDestination(newPos);
                        wanderTimer = 0;
                    }
                }
            }
        }

        public override void ExitState()
        {
            Agent.Controller.OnAttributeChange -= HandleAttributeChange;
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