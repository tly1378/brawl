using UnityEngine;
using System.Collections.Generic;

namespace Brawl
{
    public class HealingZone : MonoBehaviour
    {
        public int factionId; // 阵营编号
        public float healAmountPerSecond = 10f; // 每秒恢复的血量

        private readonly List<Health> healthTargets = new();

        void OnTriggerEnter(Collider other)
        {
            Controller health = other.GetComponent<Controller>();
            if (health != null && !healthTargets.Contains(health.Health))
            {
                if (health.factionId == factionId)
                {
                    healthTargets.Add(health.Health);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            Controller health = other.GetComponent<Controller>();
            if (health != null && healthTargets.Contains(health.Health))
            {
                healthTargets.Remove(health.Health);
            }
        }

        void Update()
        {
            foreach (Health health in healthTargets)
            {
                health.Heal(healAmountPerSecond * Time.deltaTime);
            }
        }
    }
}