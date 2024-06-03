using UnityEngine;
using UnityEngine.AI;
using System;

public class AgentController : MonoBehaviour
{
    public const float WanderRadius = 10f;
    public const float WanderInterval = 5f;
    public const float ViewRadius = 15f;
    public const float HealDistance = 1f;

    public event Action<AgentState> OnStateChange;
    public Transform Target { get; set; }

    public Transform UIPosition;
    public Transform HealPoint;

    private NavMeshAgent agent;
    private Health health;
    private AgentState currentState;

    public NavMeshAgent Agent => agent;
    public Health Health => health;

    public bool IsAlive => health.CurrentHealth > 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        health.OnDead += OnDeath;
    }

    private void OnDeath()
    {
        TransitionToState(new DeadState());
    }

    private async void Start()
    {
        await UIManager.Instance.CreateStateBar(this);
        TransitionToState(new PatrolState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void TransitionToState(AgentState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        OnStateChange?.Invoke(currentState);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ViewRadius);
    }
}
