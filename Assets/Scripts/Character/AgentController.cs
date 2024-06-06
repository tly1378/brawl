using UnityEngine;
using System;
using Brawl.State;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class AgentController : MonoBehaviour
    {
        public event Action<AgentState> OnStateChange;
        
        public Transform HealPoint;
        
        private AgentState currentState;

        public Controller Controller { get; private set; }

        public bool IsMelee { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<Controller>();
        }

        private async void Start()
        {
            await UI.UIManager.Instance.CreateOverheadUI(this);
            Controller.Health.OnDead += TransitionToDeadState;
            TransitionToState(new PatrolState(this));
            IsMelee = Controller.Attack is MeleeAttack;
        }

        private void TransitionToDeadState(Health _)
        {
            TransitionToState(new DeadState(this));
        }

        private void OnDestroy()
        {
            Controller.Health.OnDead -= TransitionToDeadState;
        }

        private void Update()
        {
            currentState?.UpdateState();
        }

        public void TransitionToState(AgentState newState)
        {
            if (newState == null)
                return;

            currentState?.ExitState();
            currentState = newState;
            currentState.EnterState();
            OnStateChange?.Invoke(currentState);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, PatrolState.ViewRadius);
        }
    }
}