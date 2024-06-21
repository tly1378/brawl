using UnityEngine;

namespace Brawl.State
{
    public abstract class PatrolState : AgentState
    {
        public const float VIEW_RADIUS = 15f;
        protected const float WANDER_INTERVAL = 5f;
        protected float wanderTimer;
        protected Vector3 originalPosition;
        protected readonly Collider[] hitColliders = new Collider[5];

        public float WanderRadius { get; set; } = 10f;

        public float MaxChaseRange { get; set; } = 15f;

        public PatrolState(AgentController agent) : base(agent)
        {
            OnUpdateState += CheckHPToHeal;
        }

        public override void EnterState()
        {
            originalPosition = Agent.transform.position;
            wanderTimer = WANDER_INTERVAL;
        }

        private string CheckHPToHeal()
        {
            var health = Agent.Controller.Health;
            if (health.CurrentHealth < health.MaxHealth)
            {
                return nameof(HealState);
            }
            else
            {
                return null;
            }
        }
    }
}
