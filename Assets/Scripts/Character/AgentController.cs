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
        public Dictionary<string, AgentState> stateDict;

        public Controller Controller { get; private set; }
        public AgentState CurrentState { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<Controller>();
        }

        private async void Start()
        {
            stateDict = new()
            {
                {nameof(PlayerState), new PlayerState(this)},
                {nameof(DeadState), new DeadState(this)},
                {nameof(FollowState), new FollowState(this)},
                {nameof(HealState), new HealState(this)},
            };
            if (Controller.Attack is MeleeAttack)
            {
                stateDict[nameof(ChaseState)] = new MeleeChaseState(this);
            }
            else if (Controller.Attack is RangedAttack)
            {
                stateDict[nameof(ChaseState)] = new RangedChaseState(this);
            }
            else if (Controller.Attack is HealAttack)
            {
                stateDict[nameof(ChaseState)] = new HealChaseState(this);
            }
            if (Controller.Attack.TargetIsFriend)
            {
                stateDict[nameof(PatrolState)] = new DoctorPatrolState(this);
            }
            else
            {
                stateDict[nameof(PatrolState)] = new SoldierPatrolState(this);
            }

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
            CurrentState?.UpdateState();
        }

        public void TransitionToState(string newState)
        {
            if (stateDict.TryGetValue(newState, out var state))
            {
                CurrentState?.ExitState();
                CurrentState = state;
                CurrentState.EnterState();
                OnStateChange?.Invoke(CurrentState);
            }
            else
            {
                Debug.LogErrorFormat("{0} doesn't exist.", newState);
            }
        }

        void OnDrawGizmosSelected()
        {
            //йср╟
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, PatrolState.VIEW_RADIUS);
        }
    }
}