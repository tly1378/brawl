using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class RangedAttack : Attack
    {
        public float projectileSpeed = 20f;

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

        private async void Attack(Health health)
        {
            lastAttackTime = Time.time;

            // 创建投掷物
            GameObject projectile = await Addressables.InstantiateAsync("Bomb", transform.position + Vector3.up, Quaternion.identity);

            // 计算投掷方向并应用力
            if (projectile.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = (health.transform.position - transform.position).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }

            // 设置投掷物的伤害
            if (projectile.TryGetComponent<Projectile>(out var projectileScript))
            {
                projectileScript.SetDamage(attackDamage);
            }
        }

        // 可视化攻击范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

