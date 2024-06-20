namespace Brawl
{
    public class Attack : UnityEngine.MonoBehaviour
    {
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackCooldown = 1f;
        protected Health target;
        protected float lastAttackTime;
        protected Controller controller;

        public Health Target
        {
            get => target;
            set
            {
                enabled = true;
                target = value;
            }
        }

        public bool TargetIsFriend => attackDamage < 0;

        private void Awake()
        {
            controller = GetComponent<Controller>();
        }
    }
}