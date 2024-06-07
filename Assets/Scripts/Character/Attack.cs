namespace Brawl
{
    public class Attack : UnityEngine.MonoBehaviour
    {
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackCooldown = 1f;
        protected Health target;
        protected float lastAttackTime;

        public Health Target
        {
            get => target;
            set
            {
                enabled = true;
                target = value;
            }
        }
    }
}