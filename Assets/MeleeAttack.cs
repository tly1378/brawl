using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public int factionId; // 阵营编号

    private float lastAttackTime;
    private AgentController enemyController;

    void Start()
    {
        enemyController = GetComponent<AgentController>();
    }

    void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            CheckForAttack();
        }
    }

    void CheckForAttack()
    {
        // 检测附近的敌人或玩家
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            Health targetHealth = hitCollider.GetComponent<Health>();
            if (targetHealth != null && targetHealth.factionId != factionId)
            {
                // 对目标造成伤害
                targetHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;

                // 设置攻击目标
                if (enemyController != null)
                {
                    enemyController.Target = hitCollider.transform;
                }

                return; // 攻击一次后退出
            }
        }
    }

    // 可视化攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
