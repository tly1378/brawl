using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float projectileSpeed = 40f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float explosionRadius = 2f;
        private static readonly Collider[] colliders = new Collider[5];

        private void Start()
        {
            Destroy(gameObject, 10f);
        }

        public async void SetDamage(float damage, Transform target)
        {
            this.damage = damage;
            await transform.DOMoveY(transform.position.y + 2, 0.5f).AsyncWaitForCompletion();
            Vector3 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * projectileSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Explode();
                Addressables.InstantiateAsync("CFXR2 WW Explosion", transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        private void Explode()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
            for (int i = 0; i < count; i++)
            {
                Collider collider = colliders[i];
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }
}
