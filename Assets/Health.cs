using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int factionId; // 阵营编号
    public Transform UIPosition;
    [SerializeField] private Transform respawnPoint; // 复活点
    private float currentHealth;

    public event Action OnDeath; // 死亡事件
    public event Action OnTakeDamage; // 受伤事件
    public event Action OnHPChange; // 受伤事件

    [ShowInInspector]
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;
            OnHPChange?.Invoke();
        }
    }

    public float MaxHealth { get; private set; } = 100;

    void Start()
    {
        CurrentHealth = MaxHealth;
        UIManager.Instance.CreateHPBar(this);
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        OnTakeDamage?.Invoke();
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if(CurrentHealth > 0)
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    }

    void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false); // 暂时禁用对象，等待复活
    }

    public void Respawn()
    {
        CurrentHealth = MaxHealth;
        transform.position = respawnPoint.position;
        gameObject.SetActive(true);
    }
}
