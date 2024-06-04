using UnityEngine;
using System;

namespace Brawl
{
    public class Health : MonoBehaviour
    {
        public int factionId; // 阵营编号
        public Transform UIPosition;
        [SerializeField] private Transform respawnPoint; // 复活点
        private float currentHealth;

        public event Action OnDead;
        public event Action OnRespawn;
        public event Action OnTakeDamage;
        public event Action OnHPChange;

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
            if (CurrentHealth > 0)
                CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        }

        void Die()
        {
            gameObject.SetActive(false);
            OnDead?.Invoke();
        }

        public void Respawn()
        {
            CurrentHealth = MaxHealth;
            transform.position = respawnPoint.position;
            gameObject.SetActive(true);
            OnRespawn?.Invoke();
        }
    }
}