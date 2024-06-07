using UnityEngine;
using System;
using Brawl.State;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class AgentController : MonoBehaviour
    {
        public event Action<AgentState> OnStateChange;

        public Transform HealPoint;

        private AgentState currentState;

        public Dictionary<StateEnum, AgentState> stateDict;

        public Controller Controller { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<Controller>();
        }

        private async void Start()
        {
            stateDict = new()
            {
                {StateEnum.Player, new PlayerState(this)},
                {StateEnum.Chase, Controller.Attack is MeleeAttack?new MeleeChaseState(this):new RangedChaseState(this)},
                {StateEnum.Dead, new DeadState(this)},
                {StateEnum.Patrol, new PatrolState(this)},
                {StateEnum.Follow, new FollowState(this)},
                {StateEnum.Heal, new HealState(this)},
            };
            Controller.Health.OnDead += TransitionToDeadState;
            await UI.UIManager.Instance.CreateOverheadUI(this);
            TransitionToState(StateEnum.Patrol);
        }

        private void TransitionToDeadState(Health _)
        {
            TransitionToState(StateEnum.Dead);
        }

        private void OnDestroy()
        {
            Controller.Health.OnDead -= TransitionToDeadState;
        }

        private void Update()
        {
            currentState?.UpdateState();
        }

        public void TransitionToState(StateEnum newState)
        {
            if(stateDict.TryGetValue(newState, out var state))
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