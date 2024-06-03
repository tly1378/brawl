using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class EnemyController : MonoBehaviour
{
    public const float WanderRadius = 10f; // 游荡半径
    public const float WanderInterval = 5f; // 每次游荡间隔时间
    public const float ViewRadius = 15f; // 视野半径
    public const float HealDistance = 1f; // 到达回血点的距离
    public const float HealthThreshold = 0.2f; // 逃跑的血量阈值
    public const float EscapeProbability = 0.5f; // 逃跑概率

    public event Action<EnemyState> OnStateChange;
    public Transform target; // 攻击目标
    public Transform healPoint; // 回血点
    public Transform UIPosition;

    private NavMeshAgent agent;
    private float wanderTimer;
    private Health health;
    private EnemyState currentState;
    private bool hasCheckedEscape; // 跟踪是否已经判断过逃跑

    public EnemyState CurrentState
    {
        get => currentState;
        private set
        {
            if (value != currentState)
            {
                OnStateChange?.Invoke(value);
                currentState = value;
            }
        }
    }

    private async void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        health.OnTakeDamage += HandleTakeDamage; // 注册受伤事件
        await UIManager.Instance.CreateStateBar(this);
        wanderTimer = WanderInterval;
        CurrentState = EnemyState.Patrol;
        hasCheckedEscape = false; // 初始值为false
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Heal:
                Heal();
                break;
        }
    }

    void Patrol()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= WanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, WanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ViewRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Health targetHealth = hitCollider.GetComponentInParent<Health>();
            if (targetHealth != null && targetHealth.factionId != health.factionId)
            {
                target = hitCollider.transform;
                CurrentState = EnemyState.Chase;
                hasCheckedEscape = false; // 重置逃跑判断
                break;
            }
        }

        if (health.CurrentHealth < health.MaxHealth)
        {
            CurrentState = EnemyState.Heal;
        }
    }

    void Chase()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth == null || targetHealth.CurrentHealth <= 0)
            {
                target = null;
                wanderTimer = WanderInterval;
                CurrentState = EnemyState.Patrol;
                hasCheckedEscape = false; // 重置逃跑判断
            }
        }
        else
        {
            wanderTimer = WanderInterval;
            CurrentState = EnemyState.Patrol;
            hasCheckedEscape = false; // 重置逃跑判断
        }
    }

    void Heal()
    {
        agent.SetDestination(healPoint.position);

        if (Vector3.Distance(transform.position, healPoint.position) < HealDistance)
        {
            if (health.CurrentHealth >= health.MaxHealth)
            {
                wanderTimer = WanderInterval;
                CurrentState = EnemyState.Patrol;
            }
        }
    }

    void HandleTakeDamage()
    {
        if (!hasCheckedEscape && health.CurrentHealth <= HealthThreshold * health.MaxHealth)
        {
            if (UnityEngine.Random.value < EscapeProbability)
            {
                CurrentState = EnemyState.Heal;
            }
            hasCheckedEscape = true; // 已经判断过逃跑
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ViewRadius);
    }
}
