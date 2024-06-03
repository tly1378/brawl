using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public float respawnTime = 5f; // 复活时间
    private readonly List<Health> deadEntities = new();

    void Start()
    {
        // 查找所有Health组件并注册死亡事件
        Health[] healthComponents = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (Health health in healthComponents)
        {
            health.OnDeath += () => HandleDeath(health);
        }
    }

    void HandleDeath(Health health)
    {
        deadEntities.Add(health);
        StartCoroutine(RespawnAfterDelay(health, respawnTime));
    }

    IEnumerator RespawnAfterDelay(Health health, float delay)
    {
        yield return new WaitForSeconds(delay);
        health.Respawn();
        deadEntities.Remove(health);
    }
}
