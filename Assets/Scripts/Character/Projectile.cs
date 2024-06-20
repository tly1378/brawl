using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class Projectile : MonoBehaviour
    {
        private static readonly Collider[] colliders = new Collider[5];
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float projectileSpeed = 40f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float explosionRadius = 2f;
        private Controller controller;

        private void Start()
        {
            Destroy(gameObject, 10f);
        }

        public async void SetDamage(float damage, Transform target, Controller owner = null)
        {
            controller = owner;
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
                if (collider.TryGetComponent<Health>(out var health))
                {
                    if (health.Controller == null || health.Controller.FactionId != controller.FactionId)
                    {
                        health.TakeDamage(damage);
                    }
                }
            }
        }
    }
}
