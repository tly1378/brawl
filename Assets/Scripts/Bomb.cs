using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Brawl
{
    public class Bomb : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 10f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Addressables.InstantiateAsync("CFXR2 WW Explosion", transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
