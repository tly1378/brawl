using UnityEngine;
using System;
using Brawl.State;
using System.Collections.Generic;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class AgentController : MonoBehaviour
    {
        public event Action<AgentState> OnStateChange;

        public Transform HealPoint;

        private AgentState currentState;

        public Dictionary<string, AgentState> stateDict;

        public Controller Controller { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<Controller>();
        }

        private async void Start()
        {
            stateDict = new()
            {
                {nameof(PlayerState), new PlayerState(this)},
                {nameof(ChaseState), Controller.Attack is MeleeAttack?new MeleeChaseState(this):new RangedChaseState(this)},
                {nameof(DeadState), new DeadState(this)},
                {nameof(PatrolState), new PatrolState(this)},
                {nameof(FollowState), new FollowState(this)},
                {nameof(HealState), new HealState(this)},
            };
            Controller.Health.OnDead += TransitionToDeadState;
            await UI.UIManager.Instance.CreateOverheadUI(this);
            TransitionToState(nameof(PatrolState));
        }

        private void TransitionToDeadState(Health _)
        {
            TransitionToState(nameof(DeadState));
        }

        private void OnDestroy()
        {
            Controller.Health.OnDead -= TransitionToDeadState;
        }

        private void Update()
        {
            currentState?.UpdateState();
        }

        public void TransitionToState(string newState)
        {
            if (stateDict.TryGetValue(newState, out var state))
            {
                currentState?.ExitState();
                currentState = state;
                currentState.EnterState();
                OnStateChange?.Invoke(currentState);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, PatrolState.ViewRadius);
        }
    }
}