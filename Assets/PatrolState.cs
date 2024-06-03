using UnityEngine;

public class PatrolState : EnemyState
{
    private float wanderTimer;
    private readonly Collider[] hitColliders = new Collider[5];

    public override void EnterState(EnemyController enemy)
    {
        wanderTimer = EnemyController.WanderInterval;
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (!enemy.IsAlive)
        {
            enemy.TransitionToState(new DeadState());
            return;
        }

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= EnemyController.WanderInterval)
        {
            Vector3 newPos = EnemyController.RandomNavSphere(enemy.transform.position, EnemyController.WanderRadius, -1);
            enemy.Agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        int count = Physics.OverlapSphereNonAlloc(enemy.transform.position, EnemyController.ViewRadius, hitColliders);
        for (int i = 0; i < count; i++)
        {
            Collider hitCollider = hitColliders[i];
            Health targetHealth = hitCollider.GetComponentInParent<Health>();
            if (targetHealth != null && targetHealth.factionId != enemy.Health.factionId)
            {
                enemy.Target = hitCollider.transform;
                enemy.TransitionToState(new ChaseState());
                break;
            }
        }

        if (enemy.Health.CurrentHealth < enemy.Health.MaxHealth)
        {
            enemy.TransitionToState(new HealState());
        }
    }
}
