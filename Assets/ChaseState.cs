public class ChaseState : EnemyState
{
    public const float HealthThreshold = 0.2f;
    public const float EscapeProbability = 0.5f;
    private bool hasCheckedEscape;
    private EnemyController enemy;

    public override void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        enemy.Health.OnTakeDamage += HandleTakeDamage;
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (!enemy.IsAlive)
        {
            enemy.TransitionToState(new DeadState());
            return;
        }

        if (enemy.Target != null)
        {
            enemy.Agent.SetDestination(enemy.Target.position);

            Health targetHealth = enemy.Target.GetComponent<Health>();
            if (targetHealth == null || targetHealth.CurrentHealth <= 0)
            {
                enemy.Target = null;
                enemy.TransitionToState(new PatrolState());
            }
        }
        else
        {
            enemy.TransitionToState(new PatrolState());
        }
    }

    private void HandleTakeDamage()
    {
        if (!hasCheckedEscape && enemy.Health.CurrentHealth <= HealthThreshold * enemy.Health.MaxHealth)
        {
            if (UnityEngine.Random.value < EscapeProbability)
            {
                enemy.TransitionToState(new HealState());
            }
            hasCheckedEscape = true;
        }
    }
}
