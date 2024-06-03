using UnityEngine;
using System.Collections.Generic;

public class HealingZone : MonoBehaviour
{
    public float healAmountPerSecond = 10f; // 每秒恢复的血量

    private List<Health> healthObjects = new List<Health>();

    void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && !healthObjects.Contains(health))
        {
            healthObjects.Add(health);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && healthObjects.Contains(health))
        {
            healthObjects.Remove(health);
        }
    }

    void Update()
    {
        foreach (Health health in healthObjects)
        {
            health.Heal(healAmountPerSecond * Time.deltaTime);
        }
    }
}
