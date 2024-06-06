using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class Projectile : MonoBehaviour
    {
        private float damage;
        public float explosionRadius = 2f;

        private void Start()
        {
            Destroy(gameObject, 10f);
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
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

        private static Collider[] colliders = new Collider[5];

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
