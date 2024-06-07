using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Brawl
{
    [RequireComponent(typeof(Health), typeof(NavMeshAgent), typeof(Attack))]
    public class Controller : MonoBehaviour
    {
        public Action<string> OnSpeak;
        [SerializeField] private int factionId;
        [SerializeField] private Transform uiPosition;
        private readonly Dictionary<string, float> attributes = new();
        private Health health;
        private Attack attack;
        private NavMeshAgent agent;

        public event Action<string, float, float?> OnAttributeChange;

        public Transform UIPosition => uiPosition;
        public int FactionId => factionId;
        public Health Health => health;
        public Attack Attack => attack;
        public NavMeshAgent Agent => agent;

        public float? GetAttribute(string name)
        {
            if (attributes.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

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
            attack = GetComponent<Attack>();
        }
    }
}