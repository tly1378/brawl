using UnityEngine;
using System;
using System.Collections.Generic;
using Brawl.State;

namespace Brawl
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AgentController))]
    public class AgentControllerEditor : ControllerEditor { }
#endif

    public class AgentController : Controller
    {
        public event Action<AgentState> OnStateChange;
        public event Action<string, float, float?> OnAttributeChange;
        public Transform HealPoint;
        private AgentState currentState;
        private readonly Dictionary<string, float> attributes = new();

        public void SetAttribute(string name, float value)
        {
            if (attributes.TryGetValue(name, out var origin))
            {
                attributes[name] = value;
                OnAttributeChange?.Invoke(name, value, origin);
            }
            else
            {
                attributes[name] = value;
                OnAttributeChange?.Invoke(name, value, null);
            }
        }

        public float? GetAttribute(string name)
        {
            if (attributes.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

        public string ShowAttributes()
        {
            string description = "";
            foreach (var attr in attributes)
            {
                description += attr.Key + "=" + attr.Value + "\n";
            }
            return description;
        }

        private async void Start()
        {
            Health.OnDead += TransitionToDeadState;
            await UI.UIManager.Instance.CreateOverheadUI(this);
            TransitionToState(new PatrolState(this));
        }

        private void TransitionToDeadState(Health _)
        {
            TransitionToState(new DeadState(this));
        }

        private void OnDestroy()
        {
            Health.OnDead -= TransitionToDeadState;
        }

        private void Update()
        {
            currentState?.UpdateState();
        }

        public void TransitionToState(AgentState newState)
        {
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