using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AgentState
{
    private float wanderTimer;
    private readonly Collider[] hitColliders = new Collider[5];

    public override void EnterState(AgentController agent)
    {
        wanderTimer = AgentController.WanderInterval;
    }

    public override void UpdateState(AgentController agent)
    {
        if (!agent.IsAlive)
        {
            agent.TransitionToState(new DeadState());
            return;
        }

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= AgentController.WanderInterval)
        {
            Vector3 newPos = RandomNavSphere(agent.transform.position, AgentController.WanderRadius, -1);
            agent.Agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        int count = Physics.OverlapSphereNonAlloc(agent.transform.position, AgentController.ViewRadius, hitColliders);
        for (int i = 0; i < count; i++)
        {
            Collider hitCollider = hitColliders[i];
            Health targetHealth = hitCollider.GetComponentInParent<Health>();
            if (targetHealth != null && targetHealth.factionId != agent.Health.factionId)
            {
                agent.Target = hitCollider.transform;
                agent.TransitionToState(new ChaseState());
                break;
            }
        }

        if (agent.Health.CurrentHealth < agent.Health.MaxHealth)
        {
            agent.TransitionToState(new HealState());
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
