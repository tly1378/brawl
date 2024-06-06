using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class MeleeAttack : Attack
    {
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

            Addressables.InstantiateAsync("CFXR Hit A (Red)", health.transform.position + Vector3.up, Quaternion.identity, health.transform);
        }

        // 可视化攻击范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}