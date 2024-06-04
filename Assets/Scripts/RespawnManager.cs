using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Brawl
{
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private float respawnTime = 5f; // 复活时间
        private Health[] healthComponents;

        private void Start()
        {
            healthComponents = FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (Health health in healthComponents)
            {
                health.OnDead += HandleDeath;
            }
        }

        private async void HandleDeath(Health health)
        {
            await UniTask.WaitForSeconds(respawnTime);
            health.Respawn();
        }

        private void OnDestroy()
        {
            foreach (Health health in healthComponents)
            {
                health.OnDead -= HandleDeath;
            }
        }
    }
}