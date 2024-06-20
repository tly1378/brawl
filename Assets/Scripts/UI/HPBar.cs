using System;
using UnityEngine;
using UnityEngine.UI;

namespace Brawl.UI
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        private Health health;


        public void Set(Health health, Color color)
        {
            this.health = health;
            health.OnHPChange += OnHPChange;
            image.color = color;
        }

        internal void Set(Health health, object value)
        {
            throw new NotImplementedException();
        }

        private void OnHPChange()
        {
            SetHP(health.CurrentHealth / health.MaxHealth);
        }

        private void SetHP(float rate)
        {
            image.fillAmount = rate;
        }
    }
}