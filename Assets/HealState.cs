using UnityEngine;

public class HealState : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.Agent.SetDestination(enemy.HealPoint.position);
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (!enemy.IsAlive)
        {
            enemy.TransitionToState(new DeadState());
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.HealPoint.position) < EnemyController.HealDistance)
        {
            if (enemy.Health.CurrentHealth >= enemy.Health.MaxHealth)
            {
                enemy.TransitionToState(new PatrolState());
            }
        }
    }
}
