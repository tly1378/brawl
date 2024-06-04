using UnityEngine;
using System;

namespace Brawl
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint; // 复活点
        private float currentHealth;

        public event Action<Health> OnDead;
        public event Action OnRespawn;
        public event Action OnTakeDamage;
        public event Action OnHPChange;

        public bool IsAlive => currentHealth > 0;

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

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;

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

        private void Die()
        {
            gameObject.SetActive(false);
            OnDead?.Invoke(this);
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