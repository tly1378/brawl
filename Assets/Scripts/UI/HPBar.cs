using UnityEngine;
using UnityEngine.UI;

namespace Brawl.UI
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        private Health health;

        public void Set(Health health)
        {
            this.health = health;
            health.OnHPChange += OnHPChange;
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