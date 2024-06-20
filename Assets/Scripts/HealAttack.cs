using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class HealAttack : Attack
    {
        private const string effectName = "CFXR3 Hit Light B (Air)";

        private void Update()
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                CheckForAttack();
            }
        }

        private void CheckForAttack()
        {
            if (Target && Target.IsAlive && Target.NeedHeal)
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

            if (!string.IsNullOrEmpty(effectName))
            {
                Addressables.InstantiateAsync(effectName, health.transform.position + Vector3.up, Quaternion.identity, health.transform);
            }
        }

        // 可视化治疗范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
