using UnityEngine;

namespace Brawl
{
    public class MeleeAttack : MonoBehaviour
    {
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackCooldown = 1f;
        public int factionId;
        private float lastAttackTime;
        private Health target;

        public Health Target
        {
            get => target;
            set
            {
                enabled = true;
                target = value;
            }
        }

        private void Update()
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                CheckForAttack();
            }
        }

        private void CheckForAttack()
        {
            if (Target && Target.IsAlive)
            {
                if (Vector3.Distance(Target.transform.position, transform.position) <= attackRange)
                {
                    Attack(Target);
                }
            }
            else
            {
                Target = null;
                enabled = false;
            }
        }

        private void Attack(Health health)
        {
            health.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }

        // 可视化攻击范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}