using UnityEngine;
using System.Collections.Generic;

public class HealingZone : MonoBehaviour
{
    public int factionId; // 阵营编号
    public float healAmountPerSecond = 10f; // 每秒恢复的血量

    private readonly List<Health> healthTargets = new();

    void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && !healthTargets.Contains(health))
        {
            if (health.factionId == factionId)
            {
                healthTargets.Add(health);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && healthTargets.Contains(health))
        {
            healthTargets.Remove(health);
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
