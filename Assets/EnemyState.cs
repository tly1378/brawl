public class EnemyState
{
    public virtual void EnterState(EnemyController enemy) { }
    public virtual void UpdateState(EnemyController enemy) { }
    public virtual void ExitState(EnemyController enemy) { }
}
