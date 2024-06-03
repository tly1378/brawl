using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    public const float WanderRadius = 10f;
    public const float WanderInterval = 5f;
    public const float ViewRadius = 15f;
    public const float HealDistance = 1f;

    public event Action<EnemyState> OnStateChange;
    public Transform Target { get; set; }

    public Transform UIPosition;
    public Transform HealPoint;

    private NavMeshAgent agent;
    private Health health;
    private EnemyState currentState;

    public NavMeshAgent Agent => agent;
    public Health Health => health;

    public bool IsAlive => health.CurrentHealth > 0;

    private async void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        await UIManager.Instance.CreateStateBar(this);
        TransitionToState(new PatrolState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void TransitionToState(EnemyState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        OnStateChange?.Invoke(currentState);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ViewRadius);
    }
}
