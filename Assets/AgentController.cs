using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

namespace Brawl
{
    public class AgentController : MonoBehaviour
    {
        public event Action<AgentState> OnStateChange;
        public event Action<string, float, float?> OnAttributeChange;
        public Transform UIPosition;
        public Transform HealPoint;
        private NavMeshAgent agent;
        private Health health;
        private AgentState currentState;
        private readonly Dictionary<string, float> attributes = new();

        public NavMeshAgent Agent => agent;
        public Health Health => health;
        public bool IsAlive => health.CurrentHealth > 0;
        public Transform Target { get; set; }

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

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        private async void Start()
        {
            await UIManager.Instance.CreateStateBar(this);
            TransitionToState(new PatrolState(this));
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