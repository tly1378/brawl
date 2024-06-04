using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Brawl
{
    public class RespawnManager : MonoBehaviour
    {
        public float respawnTime = 5f; // 复活时间

        void Start()
        {
            Health[] healthComponents = FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (Health health in healthComponents)
            {
                health.OnDead += () => HandleDeath(health);
            }
        }

        async void HandleDeath(Health health)
        {
            await UniTask.WaitForSeconds(respawnTime);
            health.Respawn();
        }
    }
}